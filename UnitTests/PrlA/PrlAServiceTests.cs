using Application.Services.PrlA;
using Domain.Contracts.PrlA;
using Domain.Enums;

namespace UnitTests.PrlA
{
    public class PrlAServiceTests
    {
        [Fact]
        public void CalcularPassivoTotalAjustado_UsaSaldoAjustadoQuandoInformado()
        {
            const decimal saldoOriginal = 5_561_968.46m;
            var contas = new List<ContaPrlADto>
            {
                new()
                {
                    Codigo = "1.01.01",
                    Ano = 2024,
                    SaldoOriginal = saldoOriginal,
                    SaldoNormalizado = saldoOriginal,
                    SaldoAjustado = null,
                    BlocoEfetivo = BlocoLiquidez.A
                }
            };

            var totalAntes = new PrlAService(null!, null!, null!, null!).CalcularPassivoTotalAjustado(contas);
            Assert.Equal(saldoOriginal, totalAntes);

            contas[0].SaldoAjustado = Math.Round(saldoOriginal * 0.9m, 2, MidpointRounding.AwayFromZero);

            var totalDepois = new PrlAService(null!, null!, null!, null!).CalcularPassivoTotalAjustado(contas);

            Assert.NotEqual(saldoOriginal, totalDepois);
            Assert.Equal(contas[0].SaldoAjustado, totalDepois);
            Assert.NotEqual(contas.Sum(c => c.SaldoOriginal), totalDepois);
        }

        [Fact]
        public void FiltrarComValor_RemoveSaldosZerados()
        {
            var contas = new[]
            {
                new ContaPrlADto { Codigo = "1.01.01", SaldoNormalizado = 10m },
                new ContaPrlADto { Codigo = "1.01.02", SaldoNormalizado = 0m },
                new ContaPrlADto { Codigo = "1.02", SaldoNormalizado = -5m }
            };

            var filtradas = PrlAService.FiltrarComValor(contas).Select(c => c.Codigo).ToList();

            Assert.Equal(new[] { "1.01.01", "1.02" }, filtradas);
        }

        [Fact]
        public void ResolverBlocoOriginal_DisponibilidadesSaoBlocoA()
        {
            Assert.Equal(BlocoLiquidez.A, PrlAService.ResolverBlocoOriginal("1.01.01"));
            Assert.Equal(BlocoLiquidez.B, PrlAService.ResolverBlocoOriginal("1.01.03"));
            Assert.Equal(BlocoLiquidez.C, PrlAService.ResolverBlocoOriginal("1.02"));
        }

        [Fact]
        public void BlocoEfetivo_UsaAjustadoQuandoPresente_SenaoOriginal()
        {
            BlocoLiquidez? ajustado = BlocoLiquidez.C;
            var original = BlocoLiquidez.A;
            var efetivo = ajustado ?? original;

            Assert.Equal(BlocoLiquidez.C, efetivo);
            Assert.Equal(BlocoLiquidez.A, ((BlocoLiquidez?)null) ?? original);
        }
    }
}
