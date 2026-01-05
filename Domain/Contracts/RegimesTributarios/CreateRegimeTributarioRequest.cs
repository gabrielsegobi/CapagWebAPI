using System.Text.Json.Serialization;

namespace Domain.Contracts.RegimesTributarios
{
    public class CreateRegimeTributarioRequest
    {
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
    }
}
