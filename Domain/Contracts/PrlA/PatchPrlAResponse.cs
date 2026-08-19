using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PatchPrlAResponse
    {
        [JsonPropertyName("conta")]
        public ContaPrlADto Conta { get; set; } = new();

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
    }
}
