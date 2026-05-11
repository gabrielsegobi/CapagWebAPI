using Application.Commands.AnalisesICP;
using Application.Commands.ResultadosIndicesICP;
using Application.Helpers;
using Application.Queries.DemonstrativosContabeis;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Json;
using Domain.Contracts.ResultadosIndicesICP;
using Domain.Entities;
using Domain.Services;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Handlers.ResultadosIndicesICP
{
    public class CalcResultadosIndicesICPHandler : IRequestHandler<CalcResultadosIndicesICPCommand, Dictionary<string, double>>
    {
        private readonly IBaseRepository<ResultadoIndiceICP> _resultadoIndiceICPRepo;
        private readonly IBaseRepository<ModeloIndiceICP> _modelosIndicesRepo;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public CalcResultadosIndicesICPHandler(
           IBaseRepository<ModeloIndiceICP> modelosIndicesRepo,
           IBaseRepository<ResultadoIndiceICP> resultadoIndiceICPRepo,
           IMediator mediator,
           ICurrentUserService currentUserService)
        {
            _modelosIndicesRepo = modelosIndicesRepo;
            _resultadoIndiceICPRepo = resultadoIndiceICPRepo;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<Dictionary<string, double>> Handle(CalcResultadosIndicesICPCommand request, CancellationToken cancellationToken)
        {
            var modelosDb = await _modelosIndicesRepo.Query().ToListAsync();
            var consolidados = await _mediator.Send(new GetDClByAnoAndCodigoQuery { IdEmpresa = request.IdEmpresa, Ano = false });

            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "Domain", "Resources", "ModelosIndicesICp.json");

            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            foreach (var modeloDb in modelosDb)
            {
                var formulaJson = formulas.FirstOrDefault(f =>
                    f.Nome.Trim().Equals(modeloDb.Nome.Trim(), StringComparison.OrdinalIgnoreCase));

                if (formulaJson != null)
                {
                    modeloDb.Formula = formulaJson.Formula;
                }
                else
                {
                    Console.WriteLine($"[AVISO] Nenhuma fórmula encontrada no JSON para o modelo '{modeloDb.Nome}'");
                }
            }

            var valoresTotais = consolidados.ToDictionary(x => x.Codigo, x => (double)x.Valor);
            var resultadosPorIndicador = new Dictionary<string, double>();
            var resultados = new List<CreateResultadoIndiceICPRequest>();
            var listaSubScores = new List<(decimal SubScore, decimal Peso)>();

            var tenantId = _currentUserService.TenantId;

            if (tenantId == null)
                throw new UnauthorizedAccessException("Tenant não identificado.");

            foreach (var formula in formulas)
            {
                var modeloDb = modelosDb.FirstOrDefault(m =>
                    m.Nome.Trim().Equals(formula.Nome.Trim(), StringComparison.OrdinalIgnoreCase));

                if (modeloDb == null)
                {
                    Console.WriteLine($"[IGNORADO] Modelo não encontrado para fórmula '{formula.Nome}'");
                    continue;
                }

                var expressao = ExpressionHelper.SubstituirCodigos(modeloDb.Formula, valoresTotais);

                double valorCalculado;
                try
                {
                    valorCalculado = ExpressionHelper.AvaliarExpressao(expressao);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERRO] Falha ao avaliar expressão!");
                    Console.WriteLine($"Expressão: {expressao}");
                    Console.WriteLine($"Mensagem: {ex.Message}");
                    valorCalculado = 0;
                }

                var subScore = CalculosServices.CalcularSubScore(modeloDb.Meta, modeloDb.PiorCaso, Convert.ToDecimal(valorCalculado));

                listaSubScores.Add((subScore, modeloDb.Peso));

                resultados.Add(new CreateResultadoIndiceICPRequest
                {
                    IdModeloIndice = modeloDb.IdModeloIndice,
                    ValorCalculado = (decimal)valorCalculado,
                    SubScoreNormalizado = subScore,
                    IdEmpresa = request.IdEmpresa,
                    IdTenant = tenantId.Value
                });

                resultadosPorIndicador[formula.Nome] = valorCalculado;
            }

            await _mediator.Send(new CreateResultadoIndiceICPCommand
            {
                CreateResultadosIndicesICPRequest = resultados
            });

            var indiceTotal = CalculosServices.CalcularIndiceTotal(listaSubScores);
            var classificacao = CalculosServices.CalcularClassificacao(indiceTotal);

            await _mediator.Send(new CreateAnaliseICPCommand
            {
                CreateAnaliseICPRequest = new CreateAnaliseICPRequest
                {
                    IdTenant = tenantId.Value,
                    IdEmpresa = request.IdEmpresa,
                    IcpCalculado = indiceTotal,
                    SomaPesos = 100m,
                    Classificacao = classificacao
                }
            });

            return resultadosPorIndicador;
        }
    }
}
