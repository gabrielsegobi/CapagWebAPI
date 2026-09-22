using System.Text.Json.Serialization;

namespace Domain.Contracts.Capag
{
    public class ContaGreManualDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("codigo_conta")]
        public string CodigoConta { get; set; } = string.Empty;

        [JsonPropertyName("codigo_pai")]
        public string? CodigoPai { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; } = string.Empty;

        [JsonPropertyName("usar_media")]
        public bool UsarMedia { get; set; }

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }

        [JsonPropertyName("valores")]
        public Dictionary<int, decimal> Valores { get; set; } = new();
    }

    public class ContaGreAjusteResponse
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("codigo_conta")]
        public string CodigoConta { get; set; } = string.Empty;

        [JsonPropertyName("gre")]
        public GreResultadoDto? Gre { get; set; }
    }

    public class ContaGreManualResponse
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("conta")]
        public ContaGreManualDto? Conta { get; set; }

        [JsonPropertyName("gre")]
        public GreResultadoDto? Gre { get; set; }
    }
}
