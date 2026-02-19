using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class UserTenantResponse
    {
        [JsonPropertyName("tenant_id")]
        public long TenantId { get; set; }
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
    }
}
