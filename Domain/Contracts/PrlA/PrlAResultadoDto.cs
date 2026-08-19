using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PrlAResultadoDto
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("ano")]
        public int? Ano { get; set; }

        [JsonPropertyName("contas")]
        public List<ContaPrlADto> Contas { get; set; } = new();

        [JsonPropertyName("total")]
        public decimal Total { get; set; }
    }
}
