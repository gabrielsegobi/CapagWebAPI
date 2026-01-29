namespace Domain.Entities
{
    public class SimulacaoIntervalo : ITenantEntity
    {
        public long IdSimulacaoIntervalo { get; set; }
        public long IdSimulacaoCalc { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string TipoIntervalo { get; set; }
        public int MesIni { get; set; }
        public int MesFim { get; set; }
        public decimal? PctMensal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public SimulacaoCalc SimulacaoCalc { get; set; }
    }
}
