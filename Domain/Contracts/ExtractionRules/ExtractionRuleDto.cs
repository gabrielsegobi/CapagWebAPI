using System.Text.Json.Serialization;

namespace Domain.Contracts.ExtractionRules
{
    public class ExtractionRuleDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("layout_id")]
        public int LayoutId { get; set; }
        [JsonPropertyName("field_label")]
        public string FieldLabel { get; set; } = string.Empty;
        [JsonPropertyName("extraction_regex")]
        public string ExtractionRegex { get; set; } = string.Empty;
        [JsonPropertyName("regex_group_index")]
        public int? RegexGroupIndex { get; set; }
        [JsonPropertyName("destination_table")]
        public string DestinationTable { get; set; } = string.Empty;
        [JsonPropertyName("destination_column")]
        public string DestinationColumn { get; set; } = string.Empty;
        [JsonPropertyName("data_type")]
        public string? DataType { get; set; } = string.Empty;
    }
}
