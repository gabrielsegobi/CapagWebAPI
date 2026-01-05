namespace Domain.Entities
{
    public class RegPgdasd
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public decimal ReceitaBruta { get; set; }
        public decimal TotalDebito { get; set; }
        public long IdFilename { get; set; }
    }
}
