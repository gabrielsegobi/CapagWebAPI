using Application.Services.Demonstrativos;
using Application.Services.SinalContabil;
using Domain.Entities;

namespace UnitTests.Demonstrativos
{
    public class DemonstrativoLeituraServiceTests
    {
        private readonly NormalizadorSinalService _normalizador = new(new GrupoContabilResolver());

        [Fact]
        public void ConsolidarDreTrimestral_SomaPeriodosComSinalDc()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 84299.70m, IndValCtaRefFin = 'C' },
                new DemonstrativoContabil { PerApur = "T02", ValCtaRefFin = 40907.90m, IndValCtaRefFin = 'C' },
                new DemonstrativoContabil { PerApur = "T03", ValCtaRefFin = 91787.66m, IndValCtaRefFin = 'D' },
                new DemonstrativoContabil { PerApur = "T04", ValCtaRefFin = 33724.67m, IndValCtaRefFin = 'D' }
            };

            var (normalizado, indicador, magnitude) =
                DemonstrativoLeituraService.ConsolidarDreTrimestral("3", items, _normalizador);

            Assert.Equal('D', indicador);
            Assert.Equal(-304.73m, normalizado);
            Assert.Equal(304.73m, magnitude);
        }

        [Fact]
        public void ConsolidarDreTrimestral_DebitoECreditoNosTrimestres()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 100m, IndValCtaRefFin = 'D' },
                new DemonstrativoContabil { PerApur = "T04", ValCtaRefFin = 150m, IndValCtaRefFin = 'C' }
            };

            var (normalizado, indicador, _) =
                DemonstrativoLeituraService.ConsolidarDreTrimestral("3", items, _normalizador);

            Assert.Equal('C', indicador);
            Assert.Equal(50m, normalizado);
        }

        [Fact]
        public void ConsolidarDreTrimestral_30101_SomaComSinalDc()
        {
            var items = new[]
            {
                new DemonstrativoContabil { PerApur = "T01", ValCtaRefFin = 10m, IndValCtaRefFin = 'D' },
                new DemonstrativoContabil { PerApur = "T04", ValCtaRefFin = 20m, IndValCtaRefFin = 'C' }
            };

            var (normalizado, indicador, magnitude) =
                DemonstrativoLeituraService.ConsolidarDreTrimestral("3.01.01.03", items, _normalizador);

            Assert.Equal(10m, normalizado);
            Assert.Equal(10m, magnitude);
            Assert.Equal('C', indicador);
        }
    }
}
