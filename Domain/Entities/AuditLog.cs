using Domain.Enums;

namespace Domain.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdUsuario { get; set; }
        public string Tabela { get; set; } = string.Empty;
        public long IdRegistro { get; set; }
        public string Acao { get; set; } = string.Empty;
        public string DadosAntigos { get; set; } = string.Empty;
        public string DadosNovos { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long CreatedYear { get; set; }
    }
}
