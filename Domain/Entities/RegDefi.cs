namespace Domain.Entities
{
    public class RegDefi : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime Periodo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public long IdFilename { get; set; }


        public RegFileName FileName { get; set; }
    }
}
