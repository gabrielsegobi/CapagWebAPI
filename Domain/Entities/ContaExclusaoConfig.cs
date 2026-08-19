namespace Domain.Entities
{
    public class ContaExclusaoConfig : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public bool Excluida { get; set; }
        public string? Justificativa { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string AtualizadoPor { get; set; } = string.Empty;
    }
}
