using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDarf
{
    public class UpdateRegDarfRequest
    {
        [JsonPropertyName("data_arrecadacao")]
        public DateOnly DataArrecadacao { get; set; }
        [JsonPropertyName("valor_total")]
        public decimal ValorTotal { get; set; }
    }
}
