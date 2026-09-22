using Application.Services.PrlA;
using Domain.Constants;
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

            var totalAntes = new PrlAService(null!, null!, null!, null!, null!).CalcularPassivoTotalAjustado(contas);
            Assert.Equal(saldoOriginal, totalAntes);

            contas[0].SaldoAjustado = Math.Round(saldoOriginal * 0.9m, 2, MidpointRounding.AwayFromZero);

            var totalDepois = new PrlAService(null!, null!, null!, null!, null!).CalcularPassivoTotalAjustado(contas);

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
        public void FiltrarComValor_MantemLinhaZeradaComAcaoOuSaldoManual()
        {
            var contas = new[]
            {
                new ContaPrlADto { Codigo = "1.01.01", SaldoNormalizado = 0m, Acao = PrlAConstants.AcaoExcluir },
                new ContaPrlADto { Codigo = "1.01.02", SaldoNormalizado = 0m, SaldoManual = 10m },
                new ContaPrlADto { Codigo = "1.01.03", SaldoNormalizado = 0m }
            };

            var filtradas = PrlAService.FiltrarComValor(contas).Select(c => c.Codigo).ToList();

            Assert.Equal(new[] { "1.01.01", "1.01.02" }, filtradas);
        }

        [Theory]
        [InlineData(100, null, 20, null, 80)]
        [InlineData(100, null, 20, PrlAConstants.AcaoIncluir, 100)]
        [InlineData(100, null, 20, PrlAConstants.AcaoIncluirComDesagio, 80)]
        [InlineData(100, null, 20, PrlAConstants.AcaoExcluir, 0)]
        [InlineData(100, 50, 20, PrlAConstants.AcaoIncluirComDesagio, 40)]
        [InlineData(100, 50, 20, PrlAConstants.AcaoIncluir, 50)]
        [InlineData(100, 50, 20, PrlAConstants.AcaoExcluir, 0)]
        public void CalcularSaldoAjustado_RespeitaAcaoESaldoManual(
            int original,
            int? manual,
            int desagio,
            string? acao,
            int esperado)
        {
            var resultado = PrlAConstants.CalcularSaldoAjustado(original, manual, desagio, acao);
            Assert.Equal(esperado, resultado);
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
