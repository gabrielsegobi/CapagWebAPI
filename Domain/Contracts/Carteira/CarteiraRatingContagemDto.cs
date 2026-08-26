using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class CarteiraRatingContagemDto
    {
        [JsonPropertyName("a")]
        public int A { get; set; }

        [JsonPropertyName("b")]
        public int B { get; set; }

        [JsonPropertyName("c")]
        public int C { get; set; }

        [JsonPropertyName("d")]
        public int D { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        public static CarteiraRatingContagemDto Empty() => new();

        public void Incrementar(string? classificacao)
        {
            switch ((classificacao ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "A": A++; break;
                case "B": B++; break;
                case "C": C++; break;
                case "D": D++; break;
                default: return;
            }

            Total = A + B + C + D;
        }

        public static CarteiraRatingContagemDto Somar(IEnumerable<CarteiraRatingContagemDto> itens)
        {
            var soma = new CarteiraRatingContagemDto();
            foreach (var item in itens)
            {
                soma.A += item.A;
                soma.B += item.B;
                soma.C += item.C;
                soma.D += item.D;
            }

            soma.Total = soma.A + soma.B + soma.C + soma.D;
            return soma;
        }
    }
}
