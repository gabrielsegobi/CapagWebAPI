using Domain.Contracts.ExtractionRules;
using Domain.Contracts.ValidatioRegexes;
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

        [JsonPropertyName("system")]
        public bool? System { get; set; }



        [JsonPropertyName("validation_regexes")]
        public List<CreateValidationRegexRequest>? ValidationRegexes { get; set; }

        //[JsonPropertyName("extraction_rules")]
        //public List<CreateExtractionRuleRequest>? ExtractionRules { get; set; }
    }
}
