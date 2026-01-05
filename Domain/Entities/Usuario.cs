namespace Domain.Entities
{
    public class Usuario
    {
        public long IdUsuario { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string SenhaHash { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime? EmailVerificadoEm { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
