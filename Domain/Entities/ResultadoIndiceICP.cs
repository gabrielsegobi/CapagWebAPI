namespace Domain.Entities
{
    public class ResultadoIndiceICP
    {
        public long IdResultadoIndice { get; set; }
        public long IdEmpresa { get; set; }
        public long IdModeloIndice { get; set; }
        public decimal? ValorCalculado { get; set; }
        public string? ValoresCalc { get; set; }
        public decimal SubScoreNormalizado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long IdTenant { get; set; }

        public ModeloIndiceICP ModeloIndice { get; set; } 

    }
}
