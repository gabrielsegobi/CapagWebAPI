using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class CreateApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
