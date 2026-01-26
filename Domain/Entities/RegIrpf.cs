namespace Domain.Entities
{
    public class RegIrpf : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public long IdFilename { get; set; }
        public decimal ValorV1 { get; set; }
        public decimal ValorV2 { get; set; }
        public decimal ValorV3 { get; set; }
        public decimal ValorV4 { get; set; }
        public decimal ValorV6 { get; set; }
        public decimal ValorV7 { get; set; }
        public string AnoCalendario { get; set; } = string.Empty;


        public RegFileName FileName { get; set; }
    }
}
