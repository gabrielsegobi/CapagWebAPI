using Application.Helpers;
using Application.Services.SinalContabil;

namespace UnitTests.Helpers
{
    public class SaldoContabilHelperTests
    {
        [Theory]
        [InlineData("1.01.01", "1.01")]
        [InlineData("3.01.01.03", "3.01.01")]
        [InlineData("1", null)]
        public void CodigoPai(string codigo, string? esperado)
        {
            Assert.Equal(esperado, SaldoContabilHelper.CodigoPai(codigo));
        }

        [Fact]
        public void ValorParaFormula_CreditoPositivo_DebitoNegativo()
        {
            Assert.Equal(-943519.63m, SaldoContabilHelper.ValorParaFormula(943519.63m, 'C', "1.01.01"));
            Assert.Equal(100m, SaldoContabilHelper.ValorParaFormula(100m, 'D', "1.01.01"));
            Assert.Equal(309752.59m, SaldoContabilHelper.ValorParaFormula(309752.59m, 'D', "1.01"));
            Assert.Equal(2628046.94m, SaldoContabilHelper.ValorParaFormula(2628046.94m, 'C', "2.01"));
            Assert.Equal(-1049174.32m, SaldoContabilHelper.ValorParaFormula(1049174.32m, 'D', "2.03"));
            Assert.Equal(1049174.32m, SaldoContabilHelper.ValorParaFormula(1049174.32m, 'C', "2.03"));
            Assert.Equal(-80m, SaldoContabilHelper.ValorParaFormula(80m, 'D', "3"));
            Assert.Equal(80m, SaldoContabilHelper.ValorParaFormula(80m, 'C', "3"));
            Assert.Equal(-80m, SaldoContabilHelper.ValorParaFormula(80m, 'D', "3.01.01"));
            Assert.Equal(80m, SaldoContabilHelper.ValorParaFormula(80m, 'C', "3.01.01"));
            Assert.Equal(-12.5m, SaldoContabilHelper.ValorParaFormula(12.5m, 'D', "3.01.01.03"));
        }

        [Theory]
        [InlineData("3.01.01", true)]
        [InlineData("3.01.01.01", true)]
        [InlineData("3.01.01.03", true)]
        [InlineData("3", false)]
        [InlineData("3.01", false)]
        [InlineData("1.01.01", false)]
        public void EhCodigoSemSinalDc(string codigo, bool esperado)
        {
            Assert.Equal(esperado, SaldoContabilHelper.EhCodigoSemSinalDc(codigo));
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("1.01.01", true)]
        [InlineData("2.01", false)]
        [InlineData("3.01.01", false)]
        public void EhContaAtivoBalanco(string codigo, bool esperado)
        {
            Assert.Equal(esperado, SaldoContabilHelper.EhContaAtivoBalanco(codigo));
        }

        [Fact]
        public void ComMagnitudeTodas_RemoveSinalDeTodasAsContas()
        {
            var valores = new Dictionary<string, double>
            {
                ["1.01.03"] = -30689503.28,
                ["3.01.01.03"] = -31712546.88,
                ["1.01.03[I]"] = -28124390.75
            };

            var result = SaldoContabilHelper.ComMagnitudeTodas(valores);

            Assert.Equal(30689503.28, result["1.01.03"]);
            Assert.Equal(31712546.88, result["3.01.01.03"]);
            Assert.Equal(28124390.75, result["1.01.03[I]"]);
        }

        [Fact]
        public void ComMagnitude30101_SoNasContas30101()
        {
            var valores = new Dictionary<string, double>
            {
                ["3"] = -50,
                ["3.01.01"] = -80,
                ["3.01.01.03"] = -12.5,
                ["1.01.01"] = -10
            };

            var result = SaldoContabilHelper.ComMagnitude30101(valores);

            Assert.Equal(-50, result["3"]);
            Assert.Equal(80, result["3.01.01"]);
            Assert.Equal(12.5, result["3.01.01.03"]);
            Assert.Equal(-10, result["1.01.01"]);
        }

        [Fact]
        public void Locator_ApontaParaAMesmaRegraDoServico()
        {
            var viaHelper = SaldoContabilHelper.ValorParaFormula(100m, 'C', "2.03");
            var viaServico = NormalizadorSinalLocator.Instance.Normalizar("2.03", 100m, 'C');
            Assert.Equal(viaServico, viaHelper);
        }
    }
}
