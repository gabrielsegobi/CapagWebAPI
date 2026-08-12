using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagCalculadoraResultados
{
    public class UpdateCapagCalculadoraResultadoRequest
    {
        [JsonPropertyName("modelo")]
        public string Modelo { get; set; } = string.Empty;

        [JsonPropertyName("classificacao")]
        public string Classificacao { get; set; } = string.Empty;

        [JsonPropertyName("percentual_exibicao")]
        public string PercentualExibicao { get; set; } = string.Empty;

        [JsonPropertyName("label_metrica")]
        public string LabelMetrica { get; set; } = string.Empty;

        [JsonPropertyName("status_mensagem")]
        public string StatusMensagem { get; set; } = string.Empty;

        [JsonPropertyName("valor_capag")]
        public decimal ValorCapag { get; set; }

        [JsonPropertyName("valor_divida")]
        public decimal? ValorDivida { get; set; }

        [JsonPropertyName("indice")]
        public decimal? Indice { get; set; }

        [JsonPropertyName("parcial")]
        public bool Parcial { get; set; }
    }
}
