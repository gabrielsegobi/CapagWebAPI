using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDirfTerceiros
{
    public class UpdateRegDirfTerceiroRequest
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [JsonPropertyName("valor_rendimento")]
        public decimal ValorRendimento { get; set; }
    }
}
