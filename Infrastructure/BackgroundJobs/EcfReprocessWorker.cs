using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Channels;

namespace Infrastructure.BackgroundJobs
{
    public class EcfReprocessWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EcfBackgroundWorker> _logger;


        private readonly Channel<long> _queue = Channel.CreateUnbounded<long>();

        // Controle de concorrência por empresa
        private readonly ConcurrentDictionary<long, SemaphoreSlim> _locks = new();
        private readonly SemaphoreSlim _signal = new(0);
        private int _isRunning = 0;

        public EcfReprocessWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<EcfBackgroundWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void Trigger(long idEmpresa)
        {
            _queue.Writer.TryWrite(idEmpresa);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Lê os itens da fila e processa um por um
            await foreach (var idEmpresa in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                // Garante que a mesma empresa não seja processada simultaneamente
                var sem = _locks.GetOrAdd(idEmpresa, _ => new SemaphoreSlim(1, 1));

                await sem.WaitAsync(stoppingToken);
                try
                {
                    await ProcessAsync(idEmpresa, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao reprocessar empresa {EmpresaId}", idEmpresa);
                }
                finally
                {
                    sem.Release();
                    _locks.TryRemove(idEmpresa, out _); // limpa o lock se ninguém estiver usando
                }
            }
        }

        private async Task<bool> ProcessAsync(long idEmpresa, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var objectStorage = scope.ServiceProvider
                .GetRequiredService<IObjectStorage>();

            var fileProcessor = scope.ServiceProvider
                .GetRequiredService<IEcfProcessorService>();

            var repo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Operation>>();
            var operationFileRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OperationFile>>();

            var operations = repo.Query(o =>
                      o.IdEmpresa == idEmpresa &&
                      o.Status == OperationStatus.ProcessadaComErro)
                  .Include(o => o.Files
                      .Where(f => f.Status == OperationFileStatus.ProcessadaComErro))
                  .ToList();

            if (!operations.Any())
                return false;

            foreach (var operation in operations)
            {
                operation.Reprocessing();
                repo.Update(operation);
                await repo.SaveChangesAsync();


                foreach (var file in operation.Files)
                {
                    file.Reprocessing();
                    operationFileRepository.Update(file);
                    await operationFileRepository.SaveChangesAsync();


                    try
                    {
                        await fileProcessor.ProcessAsync(
                            operation.Id,
                            operation.IdEmpresa,
                            operation.IdTenant,
                            file.Id,
                            file.FileName,
                            cancellationToken
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Erro ao processar arquivo {FileId} da operação {OperationId}",
                            file.Id,
                            operation.Id
                        );
                    }
                    file.Finish();
                    operationFileRepository.Update(file);
                    await operationFileRepository.SaveChangesAsync();
                }

                operation.Finish();
                repo.Update(operation);
                await repo.SaveChangesAsync();
            }

            return true;
        }
    }
}
