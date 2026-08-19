using System.Text.Json.Serialization;

namespace Domain.Contracts.ValoresAnuais
{
    public class ValorAnualDto
    {
        [JsonPropertyName("id_valor")]
        public long IdValor { get; set; }
        [JsonPropertyName("id_indicador")]
        public long IdIndicador { get; set; }
        [JsonPropertyName("ano")]
        public int Ano { get; set; }
        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }
        [JsonPropertyName("formula")]
        public string Formula { get; set; } = string.Empty;
        [JsonPropertyName("formula_contas")]
        public string FormulaContas { get; set; } = string.Empty;
        [JsonPropertyName("valores_calc_ano")]
        public string? ValoresCalcAno { get; set; }
        [JsonPropertyName("mensagem")]
        public string? Mensagem { get; set; }

        [JsonPropertyName("alerta")]
        public string? Alerta => Mensagem;
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

    }
}
