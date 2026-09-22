using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PatchJustificativaPrlARequest
    {
        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }
    }
}
