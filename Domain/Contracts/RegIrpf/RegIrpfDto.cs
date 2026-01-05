using System.Text.Json.Serialization;

namespace Domain.Contracts.RegIrpf
{
    public class RegIrpfDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("id_filename")]
        public long IdFilename { get; set; }
        [JsonPropertyName("valor_v1")]
        public decimal ValorV1 { get; set; }
        [JsonPropertyName("valor_v2")]
        public decimal ValorV2 { get; set; }
        [JsonPropertyName("valor_v3")]
        public string? ValorV3 { get; set; }
        [JsonPropertyName("valor_v4")]
        public string? ValorV4 { get; set; }
        [JsonPropertyName("valor_v6")]
        public decimal ValorV6 { get; set; }
        [JsonPropertyName("valor_v7")]
        public decimal ValorV7 { get; set; }
    }
}
