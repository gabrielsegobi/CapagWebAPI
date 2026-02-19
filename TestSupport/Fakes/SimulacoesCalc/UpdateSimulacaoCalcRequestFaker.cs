using Bogus;
using Domain.Contracts.SimulacoesCalc;

namespace TestSupport.Fakes.SimulacoesCalc
{
    public class UpdateSimulacaoCalcRequestFaker : Faker<UpdateSimulacaoCalcRequest>
    {
        public UpdateSimulacaoCalcRequestFaker()
        {
            RuleFor(x => x.TipoSimulacao, f => f.PickRandom("PREVIDENCIARIO", "DEMAIS"));
            RuleFor(x => x.LimitadorPCT, f => f.Random.Decimal(0, 30));
            RuleFor(x => x.DescMaxPct, f => f.Random.Decimal(0, 30));
            RuleFor(x => x.HasPrejuizo, f => f.Random.Bool());
            RuleFor(x => x.PrejuizoValor, (f, x) => x.HasPrejuizo ? f.Random.Decimal(1_000, 100_000) : 0);
            RuleFor(x => x.HasAbatimento, f => f.Random.Bool());
            RuleFor(x => x.AbatimentoValor, (f, x) => x.HasAbatimento ? f.Random.Decimal(500, 50_000) : 0);
        }
    }
}
