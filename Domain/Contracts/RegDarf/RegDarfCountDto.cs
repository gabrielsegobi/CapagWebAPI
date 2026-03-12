using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDarf
{
    public class RegDarfCountDto
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("valor_total")]
        public decimal ValorTotal { get; set; }
    }
}
