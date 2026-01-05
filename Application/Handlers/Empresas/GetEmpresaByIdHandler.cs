using Application.Commands.DemonstrativosContabeis;
using Application.Commands.ProcessLog;
using Application.Exceptions.Empresas;
using Application.Filters;
using Application.Queries.Empresas;
using Application.Queries.ProcessLog;
using Application.teste;
using AutoMapper;
using Domain.Contracts.Empresas;
using Domain.Contracts.ProcessLog;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public class GetEmpresaByIdHandler : IRequestHandler<GetEmpresaByIdQuery, GetApiResponse>
{
    private readonly IBaseRepository<Empresa> _baseRepository;
    private readonly IMapper _mapper;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IMediator _mediator;

    public GetEmpresaByIdHandler(
        IBaseRepository<Empresa> baseRepository,
        IMapper mapper,
        IBackgroundTaskQueue taskQueue,
        IMediator mediator)
    {
        _baseRepository = baseRepository;
        _mapper = mapper;
        _taskQueue = taskQueue;
        _mediator = mediator;
    }

    public async Task<GetApiResponse> Handle(GetEmpresaByIdQuery request, CancellationToken cancellationToken)
    {
        var empresa = await _baseRepository.GetByIdAsync(request.Id);


        if (empresa == null)
            throw new EmpresaNotFoundException(request.Id);


        if (!empresa.DadosProcessados)
        {
            var processLogs = await _mediator.Send(new GetAllProcessLogQuery(new ProcessLogFilter { IdEmpresa = empresa.IdEmpresa, }));
            if (!processLogs.Data.Any(pl => pl.Acao == "Processamento iniciado"))
            {
                var log = new CreateProcessLogRequest
                {
                    Acao = "Processamento iniciado",
                    IdEmpresa = empresa.IdEmpresa,
                    Mensagem = "",
                };

                await _mediator.Send(new CreateProcessLogCommand { CreateProcessLogRequest = log });

                _taskQueue.Enqueue(async (sp, token) =>
                {
                    Console.WriteLine($"[Fila] Iniciando processamento da empresa {empresa.IdEmpresa}");
                    try
                    {
                        var currentUser = sp.GetRequiredService<ICurrentUserService>();
                        currentUser.SetTenantId(empresa.IdTenant);
                        var mediator = sp.GetRequiredService<IMediator>();
                        var repo = sp.GetRequiredService<IBaseRepository<Empresa>>();
                        await mediator.Send(new CreateDemonstrativoContabilCommand { IdEmpresa = empresa.IdEmpresa }, token);

                        var empresaProcessada = await repo.GetByIdAsync(empresa.IdEmpresa);
                        if (empresaProcessada != null)
                        {
                            empresaProcessada.DadosProcessados = true;
                            empresaProcessada.UpdatedAt = DateTimeHelper.GetDateTimeNow();

                            repo.Update(empresaProcessada);
                            await repo.SaveChangesAsync();

                            Console.WriteLine($"[Fila] Empresa {empresa.IdEmpresa} marcada como processada com sucesso");

                            var log = new CreateProcessLogRequest
                            {
                                Acao = "Processamento terminado",
                                IdEmpresa = empresa.IdEmpresa,
                                Mensagem = "Empresa Processada com sucesso",
                            };

                            await mediator.Send(new CreateProcessLogCommand { CreateProcessLogRequest = log });

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Fila] Erro ao processar empresa {empresa.IdEmpresa}: {ex.Message}, {ex}");

                        var log = new CreateProcessLogRequest
                        {
                            Acao = "Erro ao processar Empresa",
                            IdEmpresa = empresa.IdEmpresa,
                            Mensagem = $"{ex.Message}"
                        };
                        var mediator = sp.GetRequiredService<IMediator>();
                        await mediator.Send(new CreateProcessLogCommand { CreateProcessLogRequest = log });
                    }
                });
            }
        }

        var result = _mapper.Map<EmpresaDto>(empresa);
        return new GetApiResponse
        {
            Data = result,
            Message = "Empresa encontrada com sucesso"
        };
    }
}
