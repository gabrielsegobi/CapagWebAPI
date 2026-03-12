using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class CreateApiResponse
    {
        public CreateApiResponse(string message, long id)
        {
            Message = message;
            Id = id;
        }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("id")]
        public long  Id { get; set; }
    }
}
