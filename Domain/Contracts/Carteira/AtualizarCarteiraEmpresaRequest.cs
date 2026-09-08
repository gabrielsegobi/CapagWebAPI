using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class AtualizarCarteiraEmpresaRequest
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("valor_contrato")]
        public decimal? ValorContrato { get; set; }

        [JsonPropertyName("data_impedimento")]
        public DateTime? DataImpedimento { get; set; }

        [JsonPropertyName("data_protocolo")]
        public DateTime? DataProtocolo { get; set; }
    }
}
