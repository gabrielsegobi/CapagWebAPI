using System.Text.Json.Serialization;

namespace Domain.Contracts.RegIrpf
{
    public class CreateRegIrpfRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("requests")]
        public List<RegIrpfItemRequest> Requests { get; set; } = new();
    }

    public class RegIrpfItemRequest
    {

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("ano_calendario")]
        public string AnoCalendario { get; set; } = string.Empty;

        [JsonPropertyName("valor_v1")]
        public decimal ValorV1 { get; set; }

        [JsonPropertyName("valor_v2")]
        public decimal ValorV2 { get; set; }

        [JsonPropertyName("valor_v3")]
        public decimal ValorV3 { get; set; }

        [JsonPropertyName("valor_v4")]
        public decimal ValorV4 { get; set; }

        [JsonPropertyName("valor_v6")]
        public decimal ValorV6 { get; set; }

        [JsonPropertyName("valor_v7")]
        public decimal ValorV7 { get; set; }
    }
}
