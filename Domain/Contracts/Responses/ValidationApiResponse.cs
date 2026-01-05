using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class ValidationError
    {
        [JsonPropertyName("field")]
        public string Field { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class ValidationApiResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = "Erro de validação.";

        [JsonPropertyName("errors")]
        public IEnumerable<ValidationError> Errors { get; set; } = Enumerable.Empty<ValidationError>();
    }
}
