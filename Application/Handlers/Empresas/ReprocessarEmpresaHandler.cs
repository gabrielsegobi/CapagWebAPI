using Application.Commands.DemonstrativosContabeis;
using Application.Commands.Empresas;
using Application.Commands.ProcessLog;
using Application.Exceptions.Empresas;
using Application.Filters;
using Application.Queries.ProcessLog;
using Application.teste;
using Domain.Contracts.ProcessLog;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace Application.Handlers.Empresas
{
    public class ReprocessarEmpresaHandler : IRequestHandler<ReprocessarEmpresaCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly CPGDbContext _db;
        private readonly IMediator _mediator;
        private readonly IBackgroundTaskQueue _taskQueue;

        public ReprocessarEmpresaHandler(
            IBaseRepository<Empresa> empresaRepository,
            CPGDbContext db,
            IMediator mediator,
            IBackgroundTaskQueue taskQueue)
        {
            _empresaRepository = empresaRepository;
            _db = db;
            _mediator = mediator;
            _taskQueue = taskQueue;
        }

        public async Task<UpdateApiResponse> Handle(ReprocessarEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa);
            if (empresa == null)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            var idEmpresa = empresa.IdEmpresa;
            var idTenant = empresa.IdTenant;

            var processLogs = await _mediator.Send(new GetAllProcessLogQuery(new ProcessLogFilter
            {
                IdEmpresa = idEmpresa,
                PageSize = 50,
                OrderByDescending = true
            }), cancellationToken);

            var ultimoStatus = processLogs.Data?
                .OrderByDescending(pl => pl.CreatedAt)
                .Select(pl => pl.Acao)
                .FirstOrDefault(acao =>
                    acao is "Processamento iniciado"
                        or "Processamento terminado"
                        or "Erro ao processar Empresa");

            if (ultimoStatus == "Processamento iniciado" && !empresa.DadosProcessados)
            {
                return new UpdateApiResponse
                {
                    Message = "Já existe um reprocessamento em andamento para esta empresa."
                };
            }

            await ExecutarClearRecalcAsync(idTenant, idEmpresa, cancellationToken);

            // Proc altera dados_processados fora do change tracker
            await _db.Entry(empresa).ReloadAsync(cancellationToken);

            await _mediator.Send(new CreateProcessLogCommand
            {
                CreateProcessLogRequest = new CreateProcessLogRequest
                {
                    Acao = "Processamento iniciado",
                    IdEmpresa = idEmpresa,
                    Mensagem = "Reprocessamento solicitado via API"
                }
            }, cancellationToken);

            _taskQueue.Enqueue(async (sp, token) =>
            {
                try
                {
                    var currentUser = sp.GetRequiredService<ICurrentUserService>();
                    currentUser.SetTenantId(idTenant);

                    var mediator = sp.GetRequiredService<IMediator>();
                    var repo = sp.GetRequiredService<IBaseRepository<Empresa>>();

                    await mediator.Send(new CreateDemonstrativoContabilCommand { IdEmpresa = idEmpresa }, token);

                    var empresaProcessada = await repo.GetByIdAsync(idEmpresa);
                    if (empresaProcessada == null)
                        return;

                    empresaProcessada.DadosProcessados = true;
                    empresaProcessada.UpdatedAt = DateTimeHelper.GetDateTimeNow();
                    repo.Update(empresaProcessada);
                    await repo.SaveChangesAsync();

                    await mediator.Send(new CreateProcessLogCommand
                    {
                        CreateProcessLogRequest = new CreateProcessLogRequest
                        {
                            Acao = "Processamento terminado",
                            IdEmpresa = idEmpresa,
                            Mensagem = "Empresa reprocessada com sucesso"
                        }
                    }, token);
                }
                catch (Exception ex)
                {
                    var mediator = sp.GetRequiredService<IMediator>();
                    var partes = new List<string>();
                    for (var atual = ex; atual != null; atual = atual.InnerException)
                    {
                        if (!string.IsNullOrWhiteSpace(atual.Message))
                            partes.Add(atual.Message);
                    }

                    await mediator.Send(new CreateProcessLogCommand
                    {
                        CreateProcessLogRequest = new CreateProcessLogRequest
                        {
                            Acao = "Erro ao processar Empresa",
                            IdEmpresa = idEmpresa,
                            Mensagem = string.Join(" | ", partes.Distinct())
                        }
                    }, token);
                }
            });

            return new UpdateApiResponse
            {
                Message = "Dados da empresa limpos. Recálculo iniciado em segundo plano."
            };
        }

        private async Task ExecutarClearRecalcAsync(long idTenant, long idEmpresa, CancellationToken cancellationToken)
        {
            var connection = _db.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "CALL gsaas.sp_clear_empresa_recalc(@p_tenant, @p_empresa)";
            command.CommandType = CommandType.Text;

            var pTenant = command.CreateParameter();
            pTenant.ParameterName = "@p_tenant";
            pTenant.Value = idTenant;
            command.Parameters.Add(pTenant);

            var pEmpresa = command.CreateParameter();
            pEmpresa.ParameterName = "@p_empresa";
            pEmpresa.Value = idEmpresa;
            command.Parameters.Add(pEmpresa);

            // Consome result sets da procedure (mensagem de sucesso)
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken)) { }
            while (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken)) { }
            }
        }
    }
}
