namespace Domain.Entities
{
    public class DescricaoDebito : ITenantEntity
    {
        public long IdDescricaoDebitos { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Natureza { get; set; } = string.Empty;
        public string NumCda { get; set; } = string.Empty;
        public DateTime? DataInscricao { get; set; }
        public decimal ValorPrincipal { get; set; }
        public decimal ValorMulta { get; set; }
        public decimal ValorJuros { get; set; }
        public decimal ValorEncargos { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
