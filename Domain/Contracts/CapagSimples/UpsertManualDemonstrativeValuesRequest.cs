using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagSimples
{
    public class UpsertManualDemonstrativeValuesRequest
    {
        [JsonPropertyName("demonstrativeKind")]
        public string DemonstrativeKind { get; set; } = string.Empty;

        /// <summary>
        /// Mapa conta → (ano → valor). Null ou vazio limpa o kind.
        /// </summary>
        [JsonPropertyName("porCodigo")]
        public Dictionary<string, Dictionary<string, decimal>>? PorCodigo { get; set; }
    }
}
