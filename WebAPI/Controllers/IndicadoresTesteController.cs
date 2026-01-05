using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.ValoresAnuais;
using Domain.Contracts.Views;
using Domain.Entities;
using Domain.Entities.Views;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NCalc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndicadoresTesteController : ControllerBase
    {
        private readonly IBaseViewRepository<BalancoPatrimonialVw> _bpRepository;
        private readonly IBaseViewRepository<DREVw> _dreRepository;
        private readonly IBaseRepository<Indicador> _indicadorRepo;
        private readonly IBaseRepository<ValorAnual> _valorAnualRepo;
        private readonly IMapper _mapper;

        public IndicadoresTesteController(
            IBaseViewRepository<BalancoPatrimonialVw> bpRepository,
            IBaseViewRepository<DREVw> dreRepository,
            IBaseRepository<Indicador> indicadorRepo,
            IBaseRepository<ValorAnual> valorAnualRepo,
            IMapper mapper)
        {
            _bpRepository = bpRepository;
            _dreRepository = dreRepository;
            _indicadorRepo = indicadorRepo;
            _valorAnualRepo = valorAnualRepo;
            _mapper = mapper;
        }

        [HttpGet("calcular")]
        public async Task<IActionResult> CalcularIndicadores([FromQuery] long idEmpresa)
        {

            var resultadosPorIndicador = await CalcularIndicadoresInterno(idEmpresa);

            foreach (var kv in resultadosPorIndicador)
            {
                // 🔹 Calcula a média dos valores anuais para "SaudeEmpresa"
                var mediaValores = kv.Value
                    .Select(v => (decimal?)((dynamic)v).Valor)
                    .Where(v => v.HasValue)
                    .DefaultIfEmpty(0)
                    .Average();

                // 🔹 Cria o indicador já com SaudeEmpresa
                var createIndicadorRequest = new CreateIndicadorRequest
                {
                    IdEmpresa = idEmpresa,
                    IdTenant = 1,
                    Nome = kv.Key,
                    SaudeEmpresa = mediaValores
                };

                var indicador = _mapper.Map<Indicador>(createIndicadorRequest);
                await _indicadorRepo.AddAsync(indicador);
                await _indicadorRepo.SaveChangesAsync(); // salva para gerar o Id

                // 🔹 Cria os valores anuais
                foreach (var valorObj in kv.Value)
                {
                    var valorDin = (dynamic)valorObj;
                    var createValorRequest = new CreateValorAnualRequest
                    {
                        IdIndicador = indicador.IdIndicador,
                        Ano = valorDin.Ano,
                        Valor = (decimal?)valorDin.Valor
                    };

                    var valorAnual = _mapper.Map<ValorAnual>(createValorRequest);
                    await _valorAnualRepo.AddAsync(valorAnual);
                }

                await _valorAnualRepo.SaveChangesAsync();
            }

            return Ok(new { Message = "Indicadores e valores anuais salvos com sucesso" });
        }






        private async Task<Dictionary<string, List<object>>> CalcularIndicadoresInterno(long idEmpresa)
        {
            var dataFiltro = new DateTime(2025, 10, 24, 20, 15, 36);

            // 🔹 Carrega os dados das views
            var bpList = await _bpRepository.Query()
                   .Where(x => x.IdEmpresa == idEmpresa && x.CreatedAt == dataFiltro)
                //.Where(x => x.IdEmpresa == idEmpresa) 
                .ToListAsync();

            var dreList = await _dreRepository.Query()
                 //.Where(x => x.IdEmpresa == idEmpresa)
                 .Where(x => x.IdEmpresa == idEmpresa && x.CreatedAt == dataFiltro)
                .ToListAsync();

            // 🔹 Combina os dois conjuntos
            var todos = bpList.Select(x => new
            {
                x.Codigo,
                x.Ano,
                x.ValCtaRefFin,
                x.ValCtaRefIni
            }).Concat(
                dreList.Select(x => new
                {
                    x.Codigo,
                    x.Ano,
                    x.ValCtaRefFin,
                    x.ValCtaRefIni
                })
            )
            .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
            .ToList();

            // 🔹 Códigos que exigem cálculo adicional com ValCtaRefIni
            var codigosComInicial = new HashSet<string>
            {
                "2.01.01.03",
                "1.01.02.02",
                "1.01.03"
            };

            // 🔹 Cálculo normal (ValCtaRefFin)
            var resultados = todos
                .GroupBy(x => new { x.Codigo, x.Ano })
                .Select(g => new
                {
                    Codigo = g.Key.Codigo,
                    Ano = g.Key.Ano,
                    SomaValCtaRefFin = g.Sum(x => x.ValCtaRefFin ?? 0)
                })
                .ToList();

            // 🔹 Cálculo adicional (ValCtaRefIni)
            var resultadosIniciais = todos
                .Where(x => codigosComInicial.Contains(x.Codigo))
                .GroupBy(x => new { x.Codigo, x.Ano })
                .Select(g => new
                {
                    Codigo = $"[I]{g.Key.Codigo}",
                    Ano = g.Key.Ano,
                    SomaValCtaRefFin = g.Sum(x => x.ValCtaRefIni ?? 0)
                })
                .ToList();

            // 🔹 Une os dois conjuntos
            var consolidados = resultados
                .Concat(resultadosIniciais)
                .OrderBy(x => x.Codigo)
                .ThenBy(x => x.Ano)
                .ToList();

            // 🔹 Agrupa por ano para facilitar substituição
            var valoresPorAno = consolidados
                .GroupBy(x => x.Ano)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.Codigo, x => x.SomaValCtaRefFin)
                );

            // 🔹 Carrega o JSON de fórmulas
            var jsonPath = @"C:\Users\gabriel.souza\source\repos\CapagWebAPI\Domain\Resources\Indicadores.json";
            //if (!System.IO.File.Exists(jsonPath))
            //    return NotFound($"Arquivo de fórmulas não encontrado: {jsonPath}");

            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var formulas = JsonSerializer.Deserialize<List<FormulaDto>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //if (formulas == null || !formulas.Any())
            //    return BadRequest("Nenhuma fórmula encontrada no arquivo JSON.");

            // 🔹 Função para substituir códigos contábeis pelos valores reais
            string SubstituirCodigos(string formula, Dictionary<string, double> valoresAno)
            {
                if (string.IsNullOrWhiteSpace(formula))
                    return "0";

                // 🔹 Ordena os códigos do maior para o menor (para evitar substituições parciais)
                var codigosOrdenados = valoresAno
                    .OrderByDescending(kv => kv.Key.Length)
                    .ToList();

                foreach (var kv in codigosOrdenados)
                {
                    var codigo = kv.Key;
                    var valor = kv.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    // 🔹 Escapa corretamente caracteres especiais (. e [ ])
                    var pattern = Regex.Escape(codigo)
                        .Replace(@"\[I\]", @"(?:\[I\])?"); // torna [I] opcional

                    // 🔹 Usa \b parcial para garantir que não substitua dentro de outros números
                    formula = Regex.Replace(
                        formula,
                        $@"(?<![\dA-Za-z]){pattern}(?![\dA-Za-z])",
                        valor
                    );
                }

                return formula;
            }


            // 🔹 Função para avaliar a expressão usando NCalc
            double AvaliarExpressao(string expressao)
            {
                try
                {
                    var e = new Expression(expressao);
                    var result = e.Evaluate();
                    return Convert.ToDouble(result);
                }
                catch
                {
                    return double.NaN;
                }
            }

            // 🔹 Calcula as fórmulas para cada ano
            var resultadosPorIndicador = new Dictionary<string, List<object>>();

            foreach (var ano in valoresPorAno.Keys)
            {
                var valoresAnoDouble = valoresPorAno[ano]
                    .ToDictionary(kv => kv.Key, kv => (double)kv.Value);

                foreach (var formula in formulas)
                {
                    var expressao = SubstituirCodigos(formula.Formula, valoresAnoDouble);
                    var valorCalculado = AvaliarExpressao(expressao);

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
            return resultadosPorIndicador;

        }
        private class FormulaDto
        {
            public string Nome { get; set; }
            public string Formula { get; set; }
        }
    }
}
