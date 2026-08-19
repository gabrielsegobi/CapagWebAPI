using System.Text.Json.Serialization;

namespace Domain.Contracts.PrlA
{
    public class PatchDesagioPrlARequest
    {
        [JsonPropertyName("percentual_desagio")]
        public decimal PercentualDesagio { get; set; }
    }
}
