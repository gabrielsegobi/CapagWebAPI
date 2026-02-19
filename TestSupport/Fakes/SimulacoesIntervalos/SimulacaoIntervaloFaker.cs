using Bogus;
using Domain.Entities;

namespace TestSupport.Fakes.SimulacoesIntervalos
{
    public class SimulacaoIntervaloFaker
        : Faker<SimulacaoIntervalo>
    {
        public SimulacaoIntervaloFaker()
        {
            RuleFor(x => x.IdSimulacaoIntervalo, f => f.Random.Long(1, 9999));
            RuleFor(x => x.IdSimulacaoCalc, f => 1);
            RuleFor(x => x.IdTenant, f => 1);
            RuleFor(x => x.IdEmpresa, f => 1);
            RuleFor(x => x.TipoIntervalo,f => f.PickRandom("ENTRADA", "PRESTACAO"));
            RuleFor(x => x.MesIni, f => f.Random.Int(1, 12));
            RuleFor(x => x.MesFim, (f, x) =>f.Random.Int(x.MesIni, 12));
            RuleFor(x => x.PctMensal,f => f.Random.Bool() ? f.Random.Decimal(0.1m, 10m): null);
            RuleFor(x => x.CreatedAt, f => f.Date.Past());
            RuleFor(x => x.UpdatedAt, f => f.Date.Recent());
        }

        public static SimulacaoIntervalo Entrada()
        {
            return new SimulacaoIntervaloFaker()
                .RuleFor(x => x.TipoIntervalo, _ => "ENTRADA")
                .Generate();
        }

        public static SimulacaoIntervalo Prestacao()
        {
            return new SimulacaoIntervaloFaker()
                .RuleFor(x => x.TipoIntervalo, _ => "PRESTACAO")
                .Generate();
        }
    }
}
