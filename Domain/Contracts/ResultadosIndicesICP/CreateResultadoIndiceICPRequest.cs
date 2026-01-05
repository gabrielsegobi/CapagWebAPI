using System.Text.Json.Serialization;

namespace Domain.Contracts.ResultadosIndicesICP
{
    public class CreateResultadoIndiceICPRequest
    {
        [JsonPropertyName("id_modelo_indice")]
        public long IdModeloIndice { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("valor_calculado")]
        public decimal ValorCalculado { get; set; }

        [JsonPropertyName("sub_score_normalizado")]
        public decimal SubScoreNormalizado { get; set; }
    }
}
