using System.Text.Json.Serialization;

namespace Domain.Contracts.ValidatioRegexes
{
    public class CreateValidationRegexRequest
    {
        [JsonPropertyName("regex")]
        public string Regex { get; set; } = string.Empty;
    }
}
