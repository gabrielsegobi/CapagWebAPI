using Domain.Contracts.ValidatioRegexes;
using System.Text.Json.Serialization;

namespace Domain.Contracts.DocumentsLayouts
{
    public class UpdateDocumentLayoutRequest
    {
        [JsonPropertyName("layout_name")]
        public string LayoutName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("validation_regex")]
        public string ValidationRegex { get; set; } = string.Empty;
        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        [JsonPropertyName("system")]
        public bool System { get; set; } 


        [JsonPropertyName("validation_regexes")]
        public List<UpdateValidationRegexRequest>? ValidationRegexes { get; set; }
    }
}
