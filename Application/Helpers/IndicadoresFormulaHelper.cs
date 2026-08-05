using Domain.Contracts.Indicadores;
using Domain.Contracts.Json;
using System.Text.Json;

namespace Application.Helpers
{
    public static class IndicadoresFormulaHelper
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
                "Indicadores.json");

            var jsonContent = await File.ReadAllTextAsync(jsonPath, cancellationToken);
            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, JsonOptions)
                ?? [];

            return formulas
                .GroupBy(f => f.Nome.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        }

        public static void EnriquecerIndicadores(
            IEnumerable<IndicadorDto> indicadores,
            IReadOnlyDictionary<string, FormulaJson> formulas)
        {
            foreach (var indicador in indicadores)
            {
                Aplicar(indicador.Nome, indicador.ValoresAnuais, formulas,
                    (v, desc, contas) =>
                    {
                        v.Formula = desc;
                        v.FormulaContas = contas;
                    });
            }
        }

        public static void EnriquecerCalculos(
            IEnumerable<CalcIndicadoresDto> indicadores,
            IReadOnlyDictionary<string, FormulaJson> formulas)
        {
            foreach (var indicador in indicadores)
            {
                Aplicar(indicador.Nome, indicador.ValoresAnuais, formulas,
                    (v, desc, contas) =>
                    {
                        v.Formula = desc;
                        v.FormulaContas = contas;
                    });
            }
        }

        private static void Aplicar<T>(
            string nomeIndicador,
            IEnumerable<T> valores,
            IReadOnlyDictionary<string, FormulaJson> formulas,
            Action<T, string, string> setFormula)
        {
            if (!formulas.TryGetValue(nomeIndicador.Trim(), out var formula))
            {
                foreach (var valor in valores)
                    setFormula(valor, "Fórmula não encontrada", "Fórmula não encontrada");
                return;
            }

            foreach (var valor in valores)
                setFormula(valor, formula.Desc_Formula, formula.Formula);
        }
    }
}
