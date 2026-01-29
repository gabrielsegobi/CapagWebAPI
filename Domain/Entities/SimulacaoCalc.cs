namespace Domain.Entities
{
    public class SimulacaoCalc : ITenantEntity
    {
        public long IdSimulacaoCalc { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }

        public string TipoSimulacao { get; set; }
        public decimal LimitadorPCT { get; set; }
        public decimal DescMaxPct { get; set; }

        public bool HasPrejuizo { get; set; }
        public decimal PrejuizoValor { get; set; }

        public bool HasAbatimento { get; set; }
        public decimal AbatimentoValor { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<SimulacaoIntervalo> SimulacaoIntervalos { get; set; }
    }
}
