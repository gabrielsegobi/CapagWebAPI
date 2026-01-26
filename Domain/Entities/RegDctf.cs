namespace Domain.Entities
{
    public class RegDctf : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public decimal Valor { get; set; }
        public long IdFilename { get; set; }
        public string Periodo { get; set; } = string.Empty;

        public RegFileName FileName { get; set; }
    }
}
