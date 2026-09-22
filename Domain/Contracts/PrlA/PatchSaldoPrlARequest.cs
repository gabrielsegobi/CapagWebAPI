using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PatchSaldoPrlARequest
    {
        /// <summary>Null remove o override e volta ao saldo importado.</summary>
        [JsonPropertyName("saldo")]
        public decimal? Saldo { get; set; }
    }
}
