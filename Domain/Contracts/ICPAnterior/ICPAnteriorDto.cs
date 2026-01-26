using System.Text.Json.Serialization;

namespace Domain.Contracts.ICPAnterior
{
    public class ICPAnteriorDto
    {
        [JsonPropertyName("id_icp_anterior")]
        public long IdIcpAnterior { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("classificacao")]
        public char? Classificacao { get; set; }
        [JsonPropertyName("valor_icp_receita")]
        public decimal ValorICPReceita { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
