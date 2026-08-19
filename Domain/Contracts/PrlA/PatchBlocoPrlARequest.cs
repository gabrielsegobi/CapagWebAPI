using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Contracts.PrlA
{
    public class PatchBlocoPrlARequest
    {
        [JsonPropertyName("bloco")]
        public BlocoLiquidez Bloco { get; set; }
    }
}
