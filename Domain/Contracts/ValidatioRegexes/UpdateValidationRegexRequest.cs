using System.Text.Json.Serialization;

namespace Domain.Contracts.ValidatioRegexes
{
    public class UpdateValidationRegexRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("regex")]
        public string Regex { get; set; } = string.Empty;
    }
}
