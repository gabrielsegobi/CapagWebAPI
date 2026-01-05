namespace Domain.Entities
{
    public class ValorAnual
    {
        public long IdValor { get; set; }
        public long IdIndicador { get; set; }
        public int Ano { get; set; }
        public decimal? Valor { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Indicador Indicador { get; set; } = null!;

    }
}
