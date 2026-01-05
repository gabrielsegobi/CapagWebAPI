namespace Domain.Entities
{
    public class ModeloIndiceICP
    {
        public long IdModeloIndice { get; set; }
        public long? IdTenant { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Formula { get; set; } = string.Empty;
        public decimal Meta { get; set; }
        public decimal PiorCaso { get; set; }
        public decimal Peso { get; set; }
        public bool Ativo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<ResultadoIndiceICP> Resultados { get; set; } = new List<ResultadoIndiceICP>();

    }
}
