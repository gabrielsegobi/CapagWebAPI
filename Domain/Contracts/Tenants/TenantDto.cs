using System.Text.Json.Serialization;

namespace Domain.Contracts.Tenants
{
    public class TenantDto
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("plano")]
        public string Plano { get; set; } = string.Empty;

        [JsonPropertyName("data_criacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("data_expiracao")]
        public DateTime? DataExpiracao { get; set; }

        [JsonPropertyName("configuracoes")]
        public string? Configuracoes { get; set; } = string.Empty;
    }
}
