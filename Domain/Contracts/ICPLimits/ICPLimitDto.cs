using System.Text.Json.Serialization;

namespace Domain.Contracts.ICPLimits
{
    public class ICPLimitDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

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

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
