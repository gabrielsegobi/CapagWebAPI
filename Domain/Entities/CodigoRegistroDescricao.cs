namespace Domain.Entities
{
    public class CodigoRegistroDescricao : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string ExpressaoRegular { get; set; } = string.Empty;
    }
}
