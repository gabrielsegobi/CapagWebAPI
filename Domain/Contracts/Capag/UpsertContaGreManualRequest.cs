using System.Text.Json.Serialization;

namespace Domain.Contracts.Capag
{
    public class UpsertContaGreManualRequest
    {
        [JsonPropertyName("codigo_conta")]
        public string? CodigoConta { get; set; }

        [JsonPropertyName("codigo_pai")]
        public string? CodigoPai { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>receita | despesa</summary>
        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = "receita";

        [JsonPropertyName("usar_media")]
        public bool UsarMedia { get; set; }

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }

        [JsonPropertyName("valores")]
        public Dictionary<int, decimal>? Valores { get; set; }
    }
}
