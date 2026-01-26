namespace Domain.Entities
{
    public class ICPsAnterior : ITenantEntity
    {
        public long IdIcpAnterior { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public char? Classificacao { get; set; } 
        public decimal ValorICPReceita { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
