namespace Domain.Entities
{
    public class ProcessLog: ITenantEntity
    {
        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public long IdTenant { get; set; }
    }
}
