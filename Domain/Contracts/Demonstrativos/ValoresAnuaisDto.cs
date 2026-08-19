using System.Text.Json.Serialization;

namespace Domain.Contracts.Demonstrativos
{
    public class ValoresAnuaisDto
    {
        [JsonPropertyName("por_ano")]
        public Dictionary<int, decimal> PorAno { get; set; } = new();

        [JsonPropertyName("total")]
        public decimal Total => PorAno.Values.Sum();

        public decimal Obter(int ano) => PorAno.TryGetValue(ano, out var v) ? v : 0m;

        public static ValoresAnuaisDto operator -(ValoresAnuaisDto a, ValoresAnuaisDto b)
        {
            var anos = a.PorAno.Keys.Union(b.PorAno.Keys);
            var result = new ValoresAnuaisDto();
            foreach (var ano in anos)
                result.PorAno[ano] = a.Obter(ano) - b.Obter(ano);
            return result;
        }
    }
}
