using System.Text.Json.Serialization;

namespace Domain.Contracts.RefreshTokens
{
    public class CreateRefreshTokenRequest
    {
        [JsonPropertyName("id_usuario")]
        public long IdUsuario { get; set; }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; } = string.Empty;
        
    }
}
