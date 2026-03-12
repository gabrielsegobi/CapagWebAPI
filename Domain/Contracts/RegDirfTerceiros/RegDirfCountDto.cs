using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDirfTerceiros
{
    public class RegDirfCountDto
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("valor_rendimento")]
        public decimal ValorRendimento { get; set; }
        [JsonPropertyName("valor_tributo")]
        public decimal ValorTributo { get; set; }
    }
}
