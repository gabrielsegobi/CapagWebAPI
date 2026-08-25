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
            Assert.Equal(12.5m, SaldoContabilHelper.ValorParaFormula(12.5m, 'D', "3.01.01.03"));
            Assert.Equal(12.5m, SaldoContabilHelper.ValorParaFormula(12.5m, 'C', "3.01.01.03"));
            Assert.Equal(-40m, SaldoContabilHelper.ValorParaFormula(40m, 'D', "3.01.01.03.01"));
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

        [Theory]
        [InlineData("3.01.01.03", true)]
        [InlineData("3.01.01.03.01", false)]
        [InlineData("3.01.01", false)]
        [InlineData("3.01.01.01", false)]
        public void EhContaCustoSemprePositiva(string codigo, bool esperado)
        {
            Assert.Equal(esperado, SaldoContabilHelper.EhContaCustoSemprePositiva(codigo));
        }

        [Theory]
        [InlineData(SaldoContabilHelper.NomePmp, true)]
        [InlineData(SaldoContabilHelper.NomeCoberturaJuros, true)]
        [InlineData(SaldoContabilHelper.NomeCicloFinanceiro, false)]
        [InlineData("Giro do Estoque", false)]
        [InlineData("Margem Líquida", false)]
        [InlineData("Prazo Médio de Recebimento (PMR)", false)]
        public void UsaMagnitudeAbsolutaNaFormula(string nome, bool esperado)
        {
            Assert.Equal(esperado, SaldoContabilHelper.UsaMagnitudeAbsolutaNaFormula(nome));
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
        public void ValoresParaFormula_PmpUsaMagnitude_DemaisMantemSinal()
        {
            var valores = new Dictionary<string, double>
            {
                ["3"] = -50,
                ["3.01.01"] = -80,
                ["3.01.01.03"] = -12.5,
                ["1.01.01"] = -10,
                ["2.01.01.03[I]"] = -427848.06
            };

            var pmp = SaldoContabilHelper.ValoresParaFormula(valores, SaldoContabilHelper.NomePmp);
            Assert.Equal(50, pmp["3"]);
            Assert.Equal(80, pmp["3.01.01"]);
            Assert.Equal(12.5, pmp["3.01.01.03"]);
            Assert.Equal(10, pmp["1.01.01"]);
            Assert.Equal(427848.06, pmp["2.01.01.03[I]"]);

            var ciclo = SaldoContabilHelper.ValoresParaFormula(valores, SaldoContabilHelper.NomeCicloFinanceiro);
            Assert.Equal(-50, ciclo["3"]);
            Assert.Equal(-427848.06, ciclo["2.01.01.03[I]"]);

            var margem = SaldoContabilHelper.ValoresParaFormula(valores, "Margem Operacional");
            Assert.Equal(-50, margem["3"]);
            Assert.Equal(-80, margem["3.01.01"]);
            Assert.Equal(-12.5, margem["3.01.01.03"]);
            Assert.Equal(-10, margem["1.01.01"]);
            Assert.Equal(-427848.06, margem["2.01.01.03[I]"]);
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
