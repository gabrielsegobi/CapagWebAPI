using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsDctf
{
    public class UpdateRegDctfRequest
    {
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
