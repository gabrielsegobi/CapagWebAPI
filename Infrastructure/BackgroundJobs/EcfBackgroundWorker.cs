using Domain.Contracts.ProcessLog;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundJobs
{
    public class EcfBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EcfBackgroundWorker> _logger;
        private readonly SemaphoreSlim _signal = new(0);
        private int _isRunning = 0;

        public EcfBackgroundWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<EcfBackgroundWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Trigger()
        {
            if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 0)
            {
                _signal.Release();
            }

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("🚀 ECF Worker iniciado");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _signal.WaitAsync(stoppingToken);

                    bool hasMore;

                    do
                    {
                        hasMore = await ProcessAsync(stoppingToken);
                    }
                    while (hasMore && !stoppingToken.IsCancellationRequested);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro no worker ECF");
                }
                finally
                {
                    Interlocked.Exchange(ref _isRunning, 0);
                }
            }
        }

        private async Task<bool> ProcessAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var objectStorage = scope.ServiceProvider.GetRequiredService<IObjectStorage>();
            var fileProcessor = scope.ServiceProvider.GetRequiredService<IEcfProcessorService>();
            var operationRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Operation>>();
            var operationFileRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OperationFile>>();
            var logRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<ProcessLog>>();

            var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUserService>();

            var operations = operationRepository.Query(o =>
                    o.Status == OperationStatus.AguardandoProcessamento ||
                    o.Status == OperationStatus.AguardandoProcessamentoComSobrescrita)
                .Include(o => o.Files
                    .Where(f => f.Status == OperationFileStatus.AguardandoProcessamento))
                .ToList();

            if (!operations.Any())
                return false;

            foreach (var operation in operations)
            {
                int errors = 0;
                bool overwrite = operation.Status == OperationStatus.AguardandoProcessamentoComSobrescrita;
                operation.Start();
                operationRepository.Update(operation);
                await operationRepository.SaveChangesAsync();

                foreach (var file in operation.Files)
                {
                    currentUser.SetTenantId(operation.IdTenant);
                    try
                    {
                        file.Start();
                        operationFileRepository.Update(file);
                        await operationFileRepository.SaveChangesAsync();

                        await fileProcessor.ProcessAsync(
                            operation.Id,
                            operation.IdEmpresa,
                            operation.IdTenant,
                            file.Id,
                            file.FileName,
                            cancellationToken,
                            overwrite
                        );

                        file.Finish();
                        operationFileRepository.Update(file);
                        await operationFileRepository.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        file.Error();
                        operationFileRepository.Update(file);
                       

                        errors++;

                        var log = new ProcessLog(operation.IdEmpresa, "Importação de ecf", ex.Message);
                        await logRepository.AddAsync(log);
                        await logRepository.SaveChangesAsync();
                        _logger.LogError(
                            ex,
                            "Erro ao processar arquivo {FileId} da operação {OperationId}",
                            file.Id,
                            operation.Id
                        );

                        continue;
                    }

                }

                if (errors > 0)
                {
                    operation.Error();
                    operationRepository.Update(operation);
                    await operationRepository.SaveChangesAsync();
                    continue;
                }

                operation.Finish();
                operationRepository.Update(operation);
                await operationRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
