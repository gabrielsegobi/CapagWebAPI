using System.Text.Json.Serialization;

namespace Domain.Contracts.ValoresAnuais
{
    public class CreateValorAnualRequest
    {
        [JsonPropertyName("id_valor")]
        public long IdValor { get; set; }

        [JsonPropertyName("id_indicador")]
        public long IdIndicador { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("valor")]
        public decimal? Valor { get; set; }

        [JsonPropertyName("valores_calc_ano")]
        public string valoresCalcAno { get; set; } = string.Empty;
    }
}
