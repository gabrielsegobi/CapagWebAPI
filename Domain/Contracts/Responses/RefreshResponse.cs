using System.Text.Json.Serialization;

namespace Domain.Contracts.Responses
{
    public class RefreshResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("papel")]
        public string Papel { get; set; } = string.Empty;
    }
}
