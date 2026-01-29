namespace Application.Filters
{
    public class SimulacaoCalcFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }

        public string TipoSimulacao { get; set; } = string.Empty;
        public decimal? LimitadorPCT { get; set; }
        public decimal? DescMaxPct { get; set; }

        public bool? HasPrejuizo { get; set; }
        public decimal? PrejuizoValor { get; set; }

        public bool? HasAbatimento { get; set; }
        public decimal? AbatimentoValor { get; set; }
    }
}
