namespace Domain.Entities
{
    public class RegDirfTerceiro
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public decimal ValorRendimento { get; set; }
        public decimal ValorTributo { get; set; }
        public long IdFilename { get; set; }
        public string AnoCalendario { get; set; } = string.Empty;
    }
}
