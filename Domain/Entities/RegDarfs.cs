namespace Domain.Entities
{
    public class RegDarfs
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateOnly DataArrecadacao { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime? DataProcessamento { get; set; }
        public long IdFilename { get; set; }
    }
}
