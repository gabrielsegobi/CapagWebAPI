using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class TenantPermissionResponse
    {
        [JsonPropertyName("tenant_id")]
        public long TenantId { get; set; }
        [JsonPropertyName("papel")]
        public string Papel { get; set; } = string.Empty;
    }
}
