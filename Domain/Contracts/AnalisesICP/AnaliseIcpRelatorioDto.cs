using System.Text.Json.Serialization;

namespace Domain.Contracts.AnalisesICP
{
    public class AnaliseIcpRelatorioDto
    {
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        [JsonPropertyName("razao_social")]
        public string RazaoSocial { get; set; } = string.Empty;
        [JsonPropertyName("classificacao")]
        public string Classificacao { get; set; } = string.Empty;
        [JsonPropertyName("icp_calculado")]
        public string IcpCalculado { get; set; }
    }
}
