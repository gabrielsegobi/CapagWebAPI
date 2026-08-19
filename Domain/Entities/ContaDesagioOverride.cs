namespace Domain.Entities
{
    public class ContaDesagioOverride : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public decimal PercentualDesagio { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string AtualizadoPor { get; set; } = string.Empty;
    }
}
