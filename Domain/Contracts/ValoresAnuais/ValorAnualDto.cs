using System.Text.Json.Serialization;

namespace Domain.Contracts.ValoresAnuais
{
    public class ValorAnualDto
    {
        [JsonPropertyName("id_valor")]
        public long IdValor { get; set; }
        [JsonPropertyName("id_indicador")]
        public long IdIndicador { get; set; }
        [JsonPropertyName("ano")]
        public int Ano { get; set; }
        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

    }
}
