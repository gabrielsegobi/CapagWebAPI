using Application.Commands.Indicadores;
using Application.Helpers;
using Application.Queries.DemonstrativosContabeis;
using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Json;
using Domain.Contracts.ValoresAnuais;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using System.Text.Json;

namespace Application.Handlers.Indicadores
{
    public class CalcIndicadoresHandler : IRequestHandler<CalcIndicadoresCommand, Dictionary<string, List<object>>>
    {
        private readonly IBaseRepository<Indicador> _indicadorRepo;
        private readonly IBaseRepository<ValorAnual> _valorAnualRepo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        public CalcIndicadoresHandler(IMediator mediator, IMapper mapper, IBaseRepository<Indicador> indicadorRepo, IBaseRepository<ValorAnual> valorAnualRepo, ICurrentUserService currentUserService)
        {
            _indicadorRepo = indicadorRepo;
            _valorAnualRepo = valorAnualRepo;
            _mediator = mediator;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Dictionary<string, List<object>>> Handle(CalcIndicadoresCommand request, CancellationToken cancellationToken)
        {
            var anosCalculo = await _mediator.Send(new GetAnosCalculoDemonstrativoQuery
            {
                IdEmpresa = request.IdEmpresa
            }, cancellationToken);

            // Indicadores são calculados por exercício: saldo final/inicial do BP (T04/A00)
            // e fluxo anual da DRE — não somar períodos do balanço (SomarPeriodosNoAno).
            var consolidados = await _mediator.Send(new GetDClByAnoAndCodigoQuery
            {
                IdEmpresa = request.IdEmpresa,
                Ano = true,
                AnosFiltro = anosCalculo
            }, cancellationToken);

            var valoresPorAno = consolidados
                .Where(x => x.Ano.HasValue && anosCalculo.Contains(x.Ano.Value))
                .GroupBy(x => x.Ano)
                .ToDictionary(
                    g => g.Key ?? 0,
                    g => g.GroupBy(x => x.Codigo)
                        .Select(gr => gr.First())
                        .ToDictionary(x => x.Codigo, x => x.Valor)
                );

            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "Domain", "Resources", "Indicadores.json");
            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);

            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var resultadosPorIndicador = new Dictionary<string, List<object>>();

            foreach (var ano in anosCalculo)
            {
                var valoresAnoDouble = valoresPorAno.TryGetValue(ano, out var valoresAno)
                    ? valoresAno.ToDictionary(kv => kv.Key, kv => (double)kv.Value)
                    : new Dictionary<string, double>();

                foreach (var formula in formulas)
                {
                    var expressao = ExpressionHelper.SubstituirCodigos(formula.Formula, valoresAnoDouble);
                    var valorCalculado = ExpressionHelper.AvaliarExpressao(expressao);

                    if (double.IsNaN(valorCalculado) || double.IsInfinity(valorCalculado))
                        valorCalculado = 0;

                    if (!resultadosPorIndicador.ContainsKey(formula.Nome))
                        resultadosPorIndicador[formula.Nome] = new List<object>();

                    resultadosPorIndicador[formula.Nome].Add(new
                    {
                        Ano = ano,
                        Valor = Math.Round(valorCalculado, 6),
                        Expressao = expressao
                    });
                }
            }

            var tenantId = _currentUserService.TenantId;

            if (tenantId == null)
                throw new UnauthorizedAccessException("Tenant não identificado.");

            await LimparCalculosEmpresaHelper.LimparIndicadoresAnterioresAsync(
                _indicadorRepo,
                _valorAnualRepo,
                request.IdEmpresa,
                tenantId.Value,
                cancellationToken);

            foreach (var kv in resultadosPorIndicador)
            {
                var mediaValores = kv.Value
                    .Select(v => (decimal?)((dynamic)v).Valor)
                    .Where(v => v.HasValue)
                    .DefaultIfEmpty(0)
                    .Average();

                var valoresDetalhados = kv.Value
                    .Where(v => ((decimal?)((dynamic)v).Valor).HasValue)
                    .Select(v =>
                    {
                        dynamic d = v;
                        return $"Valor{d.Ano}: {((decimal)d.Valor).ToString(System.Globalization.CultureInfo.InvariantCulture)}";
                    })
                    .ToList();

                var quantidade = valoresDetalhados.Count;

                string formula;

                if (quantidade > 0)
                {
                    var somaString = string.Join(" + ", valoresDetalhados);
                    formula = $" ({somaString}) / {quantidade}";
                }
                else
                {
                    formula = " 0";
                }

                var createIndicadorRequest = new CreateIndicadorRequest
                {
                    IdEmpresa = request.IdEmpresa,
                    IdTenant = tenantId.Value,
                    Nome = kv.Key,
                    SaudeEmpresa = mediaValores,
                    ValoresCalcSaudeEmpresa = formula
                };

                var indicador = _mapper.Map<Indicador>(createIndicadorRequest);
                await _indicadorRepo.AddAsync(indicador);
                await _indicadorRepo.SaveChangesAsync();

                foreach (var valorObj in kv.Value)
                {
                    var valorDin = (dynamic)valorObj;
                    var createValorRequest = new CreateValorAnualRequest
                    {
                        IdIndicador = indicador.IdIndicador,
                        Ano = valorDin.Ano,
                        Valor = (decimal?)valorDin.Valor,
                        valoresCalcAno = valorDin.Expressao,
                    };

                    var valorAnual = _mapper.Map<ValorAnual>(createValorRequest);
                    await _valorAnualRepo.AddAsync(valorAnual);
                }

                await _valorAnualRepo.SaveChangesAsync();
            }

            return resultadosPorIndicador;
        }
    }
}