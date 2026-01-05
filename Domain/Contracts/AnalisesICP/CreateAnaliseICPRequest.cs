using System.Text.Json.Serialization;

namespace Domain.Contracts.AnalisesICP
{
    public class CreateAnaliseICPRequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("icp_calculado")]
        public decimal IcpCalculado { get; set; }

        [JsonPropertyName("classificacao")]
        public string Classificacao { get; set; } = string.Empty;

        [JsonPropertyName("soma_pesos")]
        public decimal SomaPesos { get; set; }
    }
}
