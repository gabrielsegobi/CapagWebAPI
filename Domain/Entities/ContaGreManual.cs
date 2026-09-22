namespace Domain.Entities
{
    public class ContaGreManual : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public string? CodigoPai { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string Tipo { get; set; } = "receita";
        public bool UsarMedia { get; set; }
        public string? Justificativa { get; set; }
        public string ValoresJson { get; set; } = "{}";
        public DateTime AtualizadoEm { get; set; }
        public string AtualizadoPor { get; set; } = string.Empty;
    }
}
