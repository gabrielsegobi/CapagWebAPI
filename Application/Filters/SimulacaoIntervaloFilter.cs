using Domain.Entities;

namespace Application.Filters
{
    public class SimulacaoIntervaloFilter : BaseFilter
    {
        public long? IdSimulacaoCalc { get; set; }
        public long? IdEmpresa { get; set; }
        public string TipoIntervalo { get; set; } = string.Empty;
        public int? MesIni { get; set; }
        public int? MesFim { get; set; }
        public decimal? PctMensal { get; set; }

    }
}
