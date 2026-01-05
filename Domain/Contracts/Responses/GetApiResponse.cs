using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class GetApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public object Data { get; set; } = new object();
    }
}
