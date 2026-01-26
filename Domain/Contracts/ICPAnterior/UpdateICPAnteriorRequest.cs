using System.Text.Json.Serialization;

namespace Domain.Contracts.ICPAnterior
{
    public class UpdateICPAnteriorRequest
    {
        [JsonPropertyName("classificacao")]
        public char? Classificacao { get; set; }
        [JsonPropertyName("valor_icp_receita")]
        public decimal ValorICPReceita { get; set; }
    }
}
