using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDefis
{
    public class RegDefisCountDto
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
