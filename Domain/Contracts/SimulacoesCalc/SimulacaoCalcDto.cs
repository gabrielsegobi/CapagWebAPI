using Domain.Contracts.SimulacoesIntervalo;
using System.Text.Json.Serialization;

namespace Domain.Contracts.SimulacoesCalc
{
    public class SimulacaoCalcDto
    {
        [JsonPropertyName("id_simulacao_calc")]
        public long IdSimulacaoCalc { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("tipo_simulacao")]
        public string TipoSimulacao { get; set; }
        [JsonPropertyName("limitador_pct")]
        public decimal LimitadorPCT { get; set; }
        [JsonPropertyName("desc_max_pct")]
        public decimal DescMaxPct { get; set; }
        [JsonPropertyName("has_prejuizo")]
        public bool HasPrejuizo { get; set; }
        [JsonPropertyName("prejuizo_valor")]
        public decimal PrejuizoValor { get; set; }
        [JsonPropertyName("has_abatimento")]
        public bool HasAbatimento { get; set; }
        [JsonPropertyName("abatimento_valor")]
        public decimal AbatimentoValor { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }


        [JsonPropertyName("intervalos")]
        public List<SimulacaoIntervaloDto> Intervalos { get; set; }
    }
}
