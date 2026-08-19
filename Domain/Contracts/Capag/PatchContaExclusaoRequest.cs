using System.Text.Json.Serialization;

namespace Domain.Contracts.Capag
{
    public class PatchContaExclusaoRequest
    {
        [JsonPropertyName("excluida")]
        public bool Excluida { get; set; }

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }
    }
}
