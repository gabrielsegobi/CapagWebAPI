using System.Text.Json.Serialization;

namespace Domain.Contracts.ResultadosIndicesICP
{
    public class ResultadoIndiceICPDto
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("id_resultado_indice")]
        public long IdResultadoIndice { get; set; }
        [JsonPropertyName("id_modelo_indice")]
        public long IdModeloIndice { get; set; }
        [JsonPropertyName("valor_calculado")]
        public decimal? ValorCalculado { get; set; }
        [JsonPropertyName("sub_score_normalizado")]
        public decimal SubScoreNormalizado { get; set; }
        [JsonPropertyName("created_at")]    
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

    }
}
