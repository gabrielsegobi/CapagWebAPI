using Domain.Contracts.Json;
using Domain.Contracts.ModelosIndicesICP;
using System.Text.Json;

namespace Application.Helpers
{
    public static class ModelosIndicesIcpFormulaHelper
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<Dictionary<string, FormulaJson>> CarregarFormulasAsync(
            CancellationToken cancellationToken = default)
        {
            var jsonPath = Path.Combine(
                AppContext.BaseDirectory,
                "Domain",
                "Resources",
                "ModelosIndicesICp.json");

            var jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, JsonOptions)
                ?? [];

            return formulas
                .GroupBy(f => f.Nome.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        }

        public static void EnriquecerModelos(
            IEnumerable<ModeloIndiceICPDto> modelos,
            IReadOnlyDictionary<string, FormulaJson> formulas)
        {
            foreach (var modelo in modelos)
            {
                if (!formulas.TryGetValue(modelo.Nome.Trim(), out var formula))
                    continue;

                modelo.DescFormula = formula.Desc_Formula;
                modelo.FormulaContas = formula.Formula;

                foreach (var resultado in modelo.Resultados)
                {
                    resultado.DescFormula = formula.Desc_Formula;
                    resultado.FormulaContas = formula.Formula;
                }
            }
        }
    }
}
