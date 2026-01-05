using Domain.Contracts.ExtractionRules;
using System.Text.Json.Serialization;

namespace Domain.Contracts.DocumentsLayouts
{
    public class CreateDocumentLayoutRequest
    {

        [JsonPropertyName("layout_name")]
        public string LayoutName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("validation_regex")]
        public string ValidationRegex { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }
        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("extraction_rules")]
        public List<CreateExtractionRuleRequest>? ExtractionRules { get; set; }
    }
}
