namespace Domain.Entities
{
    public class ContaGreInversao : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public int Ano { get; set; }
        public bool Invertido { get; set; }
        public string? Justificativa { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string AtualizadoPor { get; set; } = string.Empty;
    }
}
