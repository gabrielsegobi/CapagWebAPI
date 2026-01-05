using System.Text.Json.Serialization;

namespace Domain.Contracts.ProcessLog
{
    public class ProcessLogDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("acao")]
        public string Acao { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
    }
}
