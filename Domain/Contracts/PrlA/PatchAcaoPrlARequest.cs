using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PatchAcaoPrlARequest
    {
        [JsonPropertyName("acao")]
        public string Acao { get; set; } = string.Empty;

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }
    }
}
