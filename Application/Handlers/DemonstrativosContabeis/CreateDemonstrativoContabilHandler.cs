using Application.Commands.DemonstrativosContabeis;
using Application.Commands.Indicadores;
using Application.Commands.ResultadosIndicesICP;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using System.Transactions;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class CreateDemonstrativoContabilHandler : IRequestHandler<CreateDemonstrativoContabilCommand>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly IBaseRepository<RegimeTributario> _tributarioRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IIntegracaoDemonstrativosService _integracaoService;

        public CreateDemonstrativoContabilHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            IBaseRepository<RegimeTributario> tributarioRepository,
            IMediator mediator,
            IIntegracaoDemonstrativosService integracaoService,
            IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _tributarioRepository = tributarioRepository;
            _mediator = mediator;
            _mapper = mapper;
            _integracaoService = integracaoService;
        }

        public async Task Handle(CreateDemonstrativoContabilCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa);

            //var resultados2 = await _mediator.Send(new GetEmpresaByIdQuery
            //{
            //    Id = request.IdEmpresa
            //});

            if (empresa == null)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            try
            {

                var taskDre = _integracaoService.ObterDreAsync(empresa, cancellationToken);
                var taskBalanco = _integracaoService.ObterBalancoAsync(empresa, cancellationToken);
                var taskTributacao = _integracaoService.ObterTributacoesAsync(empresa, cancellationToken);

                await Task.WhenAll(taskDre, taskBalanco, taskTributacao);

                var dadosDRE = taskDre.Result;
                var dadosBalanco = taskBalanco.Result;
                var dadosTributacao = taskTributacao.Result;

                if ((dadosDRE == null || !dadosDRE.Any()) ||
                    (dadosBalanco == null || !dadosBalanco.Any()))
                    throw new Exception("Falha ao processar dados de DRE ou Balanço.");

                var demonstrativos = new List<DemonstrativoContabil>();
                demonstrativos.AddRange(_mapper.Map<List<DemonstrativoContabil>>(dadosDRE));
                demonstrativos.AddRange(_mapper.Map<List<DemonstrativoContabil>>(dadosBalanco));

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _demonstrativoRepository.AddRangeAsync(demonstrativos);
                    await _demonstrativoRepository.SaveChangesAsync();

                    await _tributarioRepository.AddRangeAsync(_mapper.Map<List<RegimeTributario>>(dadosTributacao));
                    await _tributarioRepository.SaveChangesAsync();

                    transaction.Complete();
                }


                var resultados = await _mediator.Send(new CalcIndicadoresCommand
                {
                    IdEmpresa = request.IdEmpresa
                });

                var resultadoss = await _mediator.Send(new CalcResultadosIndicesICPCommand
                {
                    IdEmpresa = request.IdEmpresa
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao gerar demonstrativos contábeis. Nenhum dado foi salvo. Erro: {ex.Message}.");
            }
        }

    }
}
