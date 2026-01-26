using System.Text.Json.Serialization;

namespace Domain.Contracts.ICPAnterior
{
    public class CreateICPAnteriorRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("classificacao")]
        public char? Classificacao { get; set; }
        [JsonPropertyName("valor_icp_receita")]
        public decimal ValorICPReceita { get; set; }
    }
}
