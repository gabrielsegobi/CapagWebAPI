using System.Text.Json.Serialization;

namespace Domain.Contracts
{
    public class Relationship
    {
        [JsonPropertyName("REG")]
        public string Reg { get; set; } = default!;

        [JsonPropertyName("REG_PAI")]
        public string? RegPai { get; set; }
    }
}
