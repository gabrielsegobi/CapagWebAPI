using System.Text.Json.Serialization;

namespace Domain.Contracts.UsuarioTenant
{
    public class UpdateUsuarioTenantRequest
    {
     
        [JsonPropertyName("id_usuario")]
        public long IdUsuario { get; set; }

        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("papel")]
        public string Papel { get; set; } = string.Empty;

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }

        [JsonPropertyName("data_vinculo")]
        public DateTime? DataVinculo { get; set; }
    }
}
