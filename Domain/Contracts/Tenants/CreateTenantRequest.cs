using System.Text.Json.Serialization;

namespace Domain.Contracts.Tenants
{
    public class CreateTenantRequest
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("plano")]
        public string Plano { get; set; } = string.Empty;

        [JsonPropertyName("configuracoes")]
        public string? Configuracoes { get; set; } = string.Empty;
    }
}
