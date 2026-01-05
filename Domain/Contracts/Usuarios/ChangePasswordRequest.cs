using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class ChangePasswordRequest
    {
        [JsonPropertyName("senha_atual")]
        public string SenhaAtual { get; set; } = string.Empty;
        [JsonPropertyName("nova_senha")]
        public string NovaSenha { get; set; } = string.Empty;
    }
}
