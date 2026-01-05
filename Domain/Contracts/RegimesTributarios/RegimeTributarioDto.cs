using System.Text.Json.Serialization;

namespace Domain.Contracts.RegimesTributarios
{
    public class RegimeTributarioDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("dt_ini")]
        public DateOnly? DtIni { get; set; }
        [JsonPropertyName("ano")]
        public int Ano { get; set; }
        [JsonPropertyName("raiz_cnpj")]
        public string RaizCnpj { get; set; } = string.Empty;
        [JsonPropertyName("forma_trib_completa")]
        public string? FormaTribCompleta { get; set; } = string.Empty;
        [JsonPropertyName("forma_apur_completa")]
        public string? FormaApurCompleta { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
