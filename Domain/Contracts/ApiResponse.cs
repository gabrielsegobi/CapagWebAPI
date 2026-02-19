using System.Text.Json.Serialization;

namespace Domain.Contracts
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("error")]
        public bool Error { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        [JsonPropertyName("records")]
        public T Data { get; set; } = default!;
    }
}
