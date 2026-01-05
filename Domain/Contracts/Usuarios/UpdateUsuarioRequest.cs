using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class UpdateUsuarioRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}
 