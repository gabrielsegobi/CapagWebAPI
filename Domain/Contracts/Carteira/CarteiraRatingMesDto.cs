using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class CarteiraRatingMesDto
    {
        [JsonPropertyName("mes")]
        public string Mes { get; set; } = string.Empty;

        [JsonPropertyName("total_analisado")]
        public int TotalAnalisado { get; set; }

        [JsonPropertyName("a")]
        public int A { get; set; }

        [JsonPropertyName("b")]
        public int B { get; set; }

        [JsonPropertyName("c")]
        public int C { get; set; }

        [JsonPropertyName("d")]
        public int D { get; set; }

        [JsonPropertyName("impedimento")]
        public int Impedimento { get; set; }
    }
}
