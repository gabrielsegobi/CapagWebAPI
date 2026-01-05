using System.Text.Json;

namespace Domain.Entities
{
    public class Tenant
    {
        public  long IdTenant { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Plano { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public string? Configuracoes { get; set; } 
        public DateTime? DeletedAt { get; set; }
    }
}
