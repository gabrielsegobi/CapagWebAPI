using System.Text.Json.Serialization;

namespace Domain.Contracts.Usuarios
{
    public class UsuarioDto
    {
        [JsonPropertyName("id_usuario")]
        public long IdUsuario { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
        [JsonPropertyName("email_verificado_em")]
        public DateTime? EmailVerificadoEm { get; set; }
        [JsonPropertyName("ultimo_acesso")]
        public DateTime? UltimoAcesso { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
