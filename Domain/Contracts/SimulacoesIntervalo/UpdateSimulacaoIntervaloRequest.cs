using System.Text.Json.Serialization;

namespace Domain.Contracts.SimulacoesIntervalo
{
    public class UpdateSimulacaoIntervaloRequest
    {
        [JsonPropertyName("tipo_intervalo")]
        public string TipoIntervalo { get; set; }
        [JsonPropertyName("mes_ini")]
        public int MesIni { get; set; }
        [JsonPropertyName("mes_fim")]
        public int MesFim { get; set; }
        [JsonPropertyName("pct_mensal")]
        public decimal? PctMensal { get; set; }
    }
}
