using System.Text.Json.Serialization;

namespace Domain.Contracts.SimulacoesCalc
{
    public class UpdateSimulacaoCalcRequest
    {
    
        [JsonPropertyName("tipo_simulacao")]
        public string TipoSimulacao { get; set; }
        [JsonPropertyName("limitador_pct")]
        public decimal LimitadorPCT { get; set; }
        [JsonPropertyName("desc_max_pct")]
        public decimal? DescMaxPct { get; set; }
        [JsonPropertyName("has_prejuizo")]
        public bool HasPrejuizo { get; set; }
        [JsonPropertyName("prejuizo_valor")]
        public decimal PrejuizoValor { get; set; }
        [JsonPropertyName("has_abatimento")]
        public bool HasAbatimento { get; set; }
        [JsonPropertyName("abatimento_valor")]
        public decimal AbatimentoValor { get; set; }
    }
}
