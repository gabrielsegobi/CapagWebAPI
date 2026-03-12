using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsDctf
{
    public class RegDctfCountDto
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
