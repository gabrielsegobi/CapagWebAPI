using System.Text.Json.Serialization;

namespace Domain.Contracts.Capag
{
    public class PatchContaInversaoRequest
    {
        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("invertido")]
        public bool Invertido { get; set; }

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }
    }
}
