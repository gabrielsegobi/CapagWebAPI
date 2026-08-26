using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class CarteiraRatingDto
    {
        [JsonPropertyName("meses")]
        public List<CarteiraRatingMesDto> Meses { get; set; } = [];

        [JsonPropertyName("totais")]
        public CarteiraRatingTotaisDto Totais { get; set; } = new();
    }

    public class CarteiraRatingTotaisDto
    {
        [JsonPropertyName("com_impedimento")]
        public CarteiraRatingContagemDto ComImpedimento { get; set; } = new();

        [JsonPropertyName("sem_impedimento")]
        public CarteiraRatingContagemDto SemImpedimento { get; set; } = new();

        [JsonPropertyName("total_analisado")]
        public int TotalAnalisado { get; set; }
    }
}
