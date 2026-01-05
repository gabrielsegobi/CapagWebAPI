using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class CreateUsuarioRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("senha_hash")]
        public string SenhaHash { get; set; } = string.Empty;
    }
}
