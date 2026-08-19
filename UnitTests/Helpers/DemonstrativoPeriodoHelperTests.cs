using Application.Helpers;
using Domain.Entities;

namespace UnitTests.Helpers
{
    public class DemonstrativoPeriodoHelperTests
    {
        [Fact]
        public void FechamentoParaSaldoInicial_TrimestralUsaT04()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 427848.06m },
                new DemonstrativoContabil { PerApur = "T04", ValCtaRefFin = 1186609.02m }
            };

            var fechamento = DemonstrativoPeriodoHelper.FechamentoParaSaldoInicial(items, x => x.PerApur);

            Assert.Equal("T04", fechamento?.PerApur);
            Assert.Equal(1186609.02m, fechamento?.ValCtaRefFin);
        }

        [Fact]
        public void FechamentoParaSaldoInicial_AnualUsaA00()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "A00", ValCtaRefFin = 100m }
            };

            var fechamento = DemonstrativoPeriodoHelper.FechamentoParaSaldoInicial(items, x => x.PerApur);

            Assert.Equal("A00", fechamento?.PerApur);
            Assert.Equal(100m, fechamento?.ValCtaRefFin);
        }

        [Fact]
        public void FechamentoParaSaldoInicial_TrimestralSemT04RetornaNulo()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 10m },
                new DemonstrativoContabil { PerApur = "T02", ValCtaRefFin = 20m }
            };

            var fechamento = DemonstrativoPeriodoHelper.FechamentoParaSaldoInicial(items, x => x.PerApur);

            Assert.Null(fechamento);
        }

        [Fact]
        public void FechamentoDoExercicio_TrimestralPrefereT04()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 1m },
                new DemonstrativoContabil { PerApur = "T04", ValCtaRefFin = 4m }
            };

            var fechamento = DemonstrativoPeriodoHelper.FechamentoDoExercicio(items, x => x.PerApur);

            Assert.Equal("T04", fechamento?.PerApur);
        }
    }
}
