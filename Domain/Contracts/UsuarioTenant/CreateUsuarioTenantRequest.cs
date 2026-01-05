using System.Text.Json.Serialization;

namespace Domain.Contracts.UsuarioTenant
{
    public class CreateUsuarioTenantRequest
    {
        [JsonPropertyName("id_usuario")]
        public long IdUsuario { get; set; }

        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("papel")]
        public string Papel { get; set; } = string.Empty;
    }
}
