using System.Text.Json.Serialization;

namespace Domain.Contracts.ValoresAnuais
{
    public class CalcValoresAnuaisDto
    {
        [JsonPropertyName("id_valor")]
        public long IdValor { get; set; }
        [JsonPropertyName("ano")]
        public string Ano { get; set; } = string.Empty;
        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }
        [JsonPropertyName("formula")]
        public string Formula { get; set; } = string.Empty;
        [JsonPropertyName("formula_contas")]
        public string FormulaContas { get; set; } = string.Empty;
        [JsonPropertyName("valores_calc_ano")]
        public string ValoresCalcAno { get; set; } = string.Empty;
        [JsonPropertyName("mensagem")]
        public string? Mensagem { get; set; }

        [JsonPropertyName("alerta")]
        public string? Alerta => Mensagem;
    }
}
