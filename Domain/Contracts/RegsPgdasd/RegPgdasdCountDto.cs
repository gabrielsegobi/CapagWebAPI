using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsPgdasd
{
    public class RegPgdasdCountDto
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("receita_bruta")]
        public decimal ReceitaBruta { get; set; }
        [JsonPropertyName("total_debito")]
        public decimal TotalDebito { get; set; }
    }
}
