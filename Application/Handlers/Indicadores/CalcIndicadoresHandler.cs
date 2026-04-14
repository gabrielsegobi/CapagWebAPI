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
            var consolidados = await _mediator.Send(new GetDClByAnoAndCodigoQuery { IdEmpresa = request.IdEmpresa, Ano = true });

     //       var valoresPorAno = consolidados
     //.GroupBy(x => x.Ano) // agrupa pelo ano real, não por bool
     //.ToDictionary(
     //    g => g.Key ?? 0, // ou algum valor padrão se Ano puder ser null
     //    g => g
     //        .GroupBy(x => x.Codigo) // garante unicidade
     //        //.Select(gr => gr.First()) // pega o primeiro se houver duplicados
     //        .ToDictionary(x => x.Codigo, x => x.Valor)
     //);
            var valoresPorAno = consolidados
                .GroupBy(x => x.Ano)
                .ToDictionary(
                    g => g.Key ?? 0,
                    g => g
                        .GroupBy(x => x.Codigo)
                        .ToDictionary(
                            gr => gr.Key,
                            gr => gr.Sum(x => x.Valor)
                        )
                );

            //        var valoresPorAno = consolidados
            


            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "Domain", "Resources", "Indicadores.json");
            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var resultadosPorIndicador = new Dictionary<string, List<object>>();

            foreach (var ano in valoresPorAno.Keys)
            {
                var valoresAnoDouble = valoresPorAno[ano]
                    .ToDictionary(kv => kv.Key, kv => (double)kv.Value);

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
                        Valor = valorCalculado,
                        Expressao = expressao
                    });
                }
            }

            var tenantId = _currentUserService.TenantId;

            if (tenantId == null)
                throw new UnauthorizedAccessException("Tenant não identificado.");

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








// Versão com consoles adicionados para depuração
//using Application.Commands.Indicadores;
//using Application.Helpers;
//using Application.Queries.DemonstrativosContabeis;
//using AutoMapper;
//using Domain.Contracts.Indicadores;
//using Domain.Contracts.Json;
//using Domain.Contracts.ValoresAnuais;
//using Domain.Entities;
//using Infrastructure.Interface;
//using MediatR;
//using System.Text.Json;

//namespace Application.Handlers.Indicadores
//{
//    public class CalcIndicadoresHandler : IRequestHandler<CalcIndicadoresCommand, Dictionary<string, List<object>>>
//    {
//        private readonly IBaseRepository<Indicador> _indicadorRepo;
//        private readonly IBaseRepository<ValorAnual> _valorAnualRepo;
//        private readonly IMediator _mediator;
//        private readonly IMapper _mapper;
//        private readonly ICurrentUserService _currentUserService;

//        public CalcIndicadoresHandler(IMediator mediator, IMapper mapper, IBaseRepository<Indicador> indicadorRepo, IBaseRepository<ValorAnual> valorAnualRepo, ICurrentUserService currentUserService)
//        {
//            _indicadorRepo = indicadorRepo;
//            _valorAnualRepo = valorAnualRepo;
//            _mediator = mediator;
//            _mapper = mapper;
//            _currentUserService = currentUserService;
//        }

//        public async Task<Dictionary<string, List<object>>> Handle(CalcIndicadoresCommand request, CancellationToken cancellationToken)
//        {
//            Console.WriteLine("[INICIO] Calculando indicadores...");

//            var consolidados = await _mediator.Send(new GetDClByAnoAndCodigoQuery { IdEmpresa = request.IdEmpresa, Ano = true });
//            Console.WriteLine($"[INFO] Registros consolidados recebidos: {consolidados.Count()}");

//            var valoresPorAno = consolidados
//                .GroupBy(x => x.Ano)
//                .ToDictionary(
//                    g => g.Key ?? 0,
//                    g => g
//                        .GroupBy(x => x.Codigo)
//                        .Select(gr => gr.First())
//                        .ToDictionary(x => x.Codigo, x => x.Valor)
//                );

//            foreach (var ano in valoresPorAno)
//            {
//                Console.WriteLine($"[ANO] {ano.Key} → {ano.Value.Count} códigos encontrados");
//                foreach (var item in ano.Value)
//                    Console.WriteLine($"   Código {item.Key} = {item.Value}");
//            }

//            var basePath = AppContext.BaseDirectory;
//            var jsonPath = Path.Combine(basePath, "Domain", "Resources", "Indicadores.json");
//            Console.WriteLine($"[INFO] Lendo JSON: {jsonPath}");

//            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
//            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, new JsonSerializerOptions
//            {
//                PropertyNameCaseInsensitive = true
//            });

//            Console.WriteLine($"[INFO] Formulas carregadas: {formulas.Count}");

//            var resultadosPorIndicador = new Dictionary<string, List<object>>();

//            foreach (var ano in valoresPorAno.Keys)
//            {
//                var valoresAnoDouble = valoresPorAno[ano]
//                    .ToDictionary(kv => kv.Key, kv => (double)kv.Value);

//                Console.WriteLine($"\n[CÁLCULOS PARA O ANO {ano}] ------------------------");

//                foreach (var formula in formulas)
//                {
//                    Console.WriteLine($"\n[INDICADOR] {formula.Nome}");
//                    Console.WriteLine($"Fórmula original: {formula.Formula}");

//                    var expressao = ExpressionHelper.SubstituirCodigos(formula.Formula, valoresAnoDouble);
//                    Console.WriteLine($"Expressão substituída: {expressao}");

//                    var valorCalculado = ExpressionHelper.AvaliarExpressao(expressao);
//                    Console.WriteLine($"Valor calculado: {valorCalculado}");

//                    if (double.IsNaN(valorCalculado) || double.IsInfinity(valorCalculado))
//                    {
//                        Console.WriteLine("Valor inválido detectado — ajustado para 0");
//                        valorCalculado = 0;
//                    }

//                    if (!resultadosPorIndicador.ContainsKey(formula.Nome))
//                        resultadosPorIndicador[formula.Nome] = new List<object>();

//                    resultadosPorIndicador[formula.Nome].Add(new
//                    {
//                        Ano = ano,
//                        Valor = valorCalculado,
//                        Expressao = expressao
//                    });
//                }
//            }

//            var tenantId = _currentUserService.TenantId;

//            if (tenantId == null)
//                throw new UnauthorizedAccessException("Tenant não identificado.");

//            foreach (var kv in resultadosPorIndicador)
//            {
//                Console.WriteLine($"\n[SALVANDO INDICADOR] {kv.Key}");

//                var mediaValores = kv.Value
//                    .Select(v => (decimal?)((dynamic)v).Valor)
//                    .Where(v => v.HasValue)
//                    .DefaultIfEmpty(0)
//                    .Average();

//                Console.WriteLine($"Média dos valores: {mediaValores}");

//                var createIndicadorRequest = new CreateIndicadorRequest
//                {
//                    IdEmpresa = request.IdEmpresa,
//                    IdTenant = tenantId.Value,
//                    Nome = kv.Key,
//                    SaudeEmpresa = mediaValores
//                };

//                var indicador = _mapper.Map<Indicador>(createIndicadorRequest);
//                await _indicadorRepo.AddAsync(indicador);
//                await _indicadorRepo.SaveChangesAsync();

//                Console.WriteLine($"Indicador criado com ID: {indicador.IdIndicador}");

//                foreach (var valorObj in kv.Value)
//                {
//                    var valorDin = (dynamic)valorObj;
//                    Console.WriteLine($"Salvando valor anual → Ano: {valorDin.Ano}, Valor: {valorDin.Valor}");

//                    var createValorRequest = new CreateValorAnualRequest
//                    {
//                        IdIndicador = indicador.IdIndicador,
//                        Ano = valorDin.Ano,
//                        Valor = (decimal?)valorDin.Valor
//                    };

//                    var valorAnual = _mapper.Map<ValorAnual>(createValorRequest);
//                    await _valorAnualRepo.AddAsync(valorAnual);
//                }

//                await _valorAnualRepo.SaveChangesAsync();
//            }

//            Console.WriteLine("\n[FIM] Cálculo concluído.");

//            return resultadosPorIndicador;
//        }
//    }
//}
