using System.Text.Json.Serialization;

namespace Domain.Contracts.AnalisesICP
{
    public class AnaliseICPDto
    {
        [JsonPropertyName("id_analise")]
        public long IdAnalise { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("icp_calculado")]
        public decimal IcpCalculado { get; set; }
        [JsonPropertyName("classificacao")]
        public string? Classificacao { get; set; } = string.Empty;
        [JsonPropertyName("soma_pesos")]
        public decimal SomaPesos { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
