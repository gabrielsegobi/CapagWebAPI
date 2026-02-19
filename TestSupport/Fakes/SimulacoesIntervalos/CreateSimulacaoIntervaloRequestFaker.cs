using Bogus;
using Domain.Contracts.SimulacoesIntervalo;
using Domain.Entities;

namespace TestSupport.Fakes.SimulacoesIntervalos
{
    public class CreateSimulacaoIntervaloRequestFaker
    : Faker<CreateSimulacaoIntervaloRequest>
    {
        public CreateSimulacaoIntervaloRequestFaker()
        {
            RuleFor(x => x.IdSimulacaoCalc, f => 1);
            RuleFor(x => x.IdEmpresa, f => 1);

            RuleFor(x => x.TipoIntervalo, f => f.PickRandom("ENTRADA", "PRESTACAO"));

            RuleFor(x => x.MesIni, f => f.Random.Int(1, 12));

            RuleFor(x => x.MesFim, (f, x) =>
                f.Random.Int(x.MesIni, 12));

            RuleFor(x => x.PctMensal, f => f.Random.Decimal(0.1m, 10m));
        }

        public static CreateSimulacaoIntervaloRequest Entrada()
        {
            return new CreateSimulacaoIntervaloRequestFaker()
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
