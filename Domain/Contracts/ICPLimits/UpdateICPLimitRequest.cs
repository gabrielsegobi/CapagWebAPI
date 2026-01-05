using System.Text.Json.Serialization;

namespace Domain.Contracts.ICPLimits
{
    public class UpdateICPLimitRequest
    {

        [JsonPropertyName("label")]
        public string? Label { get; set; } = string.Empty;

        [JsonPropertyName("min_value")]
        public decimal? MinValue { get; set; }

        [JsonPropertyName("max_value")]
        public decimal? MaxValue { get; set; }

        [JsonPropertyName("sort_order")]
        public byte? SortOrder { get; set; }

        [JsonPropertyName("color_code")]
        public string ColorCode { get; set; } = string.Empty;

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}
