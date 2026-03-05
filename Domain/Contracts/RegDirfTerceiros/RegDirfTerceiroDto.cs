using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDirfTerceiros
{
    public class RegDirfTerceiroDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
      
        [JsonPropertyName("id_filename")]
        public long IdFilename { get; set; }
      
        [JsonPropertyName("data_processamento")]
        public DateTime DataProcessamento { get; set; }
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [JsonPropertyName("valor_rendimento")]
        public decimal ValorRendimento { get; set; }
        [JsonPropertyName("valor_tributo")]
        public decimal ValorTributo { get; set; }
    }
}
