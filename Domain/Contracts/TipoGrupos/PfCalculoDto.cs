using System.Text.Json.Serialization;

namespace Domain.Contracts.TipoGrupos
{
    public class PfCalculoDto
    {
        [JsonPropertyName("valor_v1")]
        public decimal ValorV1 { get; set; }
        [JsonPropertyName("valor_v2")]
        public decimal ValorV2 { get; set; }
        [JsonPropertyName("valor_v3")]
        public decimal ValorV3 { get; set; }
        [JsonPropertyName("valor_v4")]
        public decimal ValorV4 { get; set; }
        [JsonPropertyName("valor_v6")]
        public decimal ValorV6 { get; set; }
        [JsonPropertyName("valor_v7")]
        public decimal ValorV7 { get; set; }
    }
}
