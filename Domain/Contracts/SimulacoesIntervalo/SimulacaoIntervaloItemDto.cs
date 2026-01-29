using System.Text.Json.Serialization;

namespace Domain.Contracts.SimulacoesIntervalo
{
    public class SimulacaoIntervaloItemDto
    {
        [JsonPropertyName("id_simulacao_intervalo")]
        public long IdSimulacaoIntervalo { get; set; }
        [JsonPropertyName("id_simulacao_calc")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("tipo_intervalo")]
        public string TipoIntervalo { get; set; }
        [JsonPropertyName("mes_ini")]
        public int MesIni { get; set; }
        [JsonPropertyName("mes_fim")]
        public int MesFim { get; set; }
        [JsonPropertyName("pct_mensal")]
        public decimal? PctMensal { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
