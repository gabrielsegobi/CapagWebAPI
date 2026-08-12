using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagSimples
{
    public class ManualDemonstrativeValuesResponse
    {
        [JsonPropertyName("dre")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, Dictionary<string, decimal>>? Dre { get; set; }

        [JsonPropertyName("balanceSheet")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, Dictionary<string, decimal>>? BalanceSheet { get; set; }
    }
}
