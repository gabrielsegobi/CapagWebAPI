using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDarf
{
    public class RegDarfDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_filename")]
        public long IdFilename { get; set; }
        
        [JsonPropertyName("data_arrecadacao")]
        public DateOnly DataArrecadacao { get; set; }
        [JsonPropertyName("valor_total")]
        public decimal ValorTotal { get; set; }
        [JsonPropertyName("data_processamento")]
        public DateTime? DataProcessamento { get; set; }
    }
}
