using Application.Helpers;

namespace UnitTests.Helpers
{
    public class IndicadorAlertaPlHelperTests
    {
        [Fact]
        public void Roe_ComPlNegativo_OmiteCalculoEUsaAlerta()
        {
            var valores = new Dictionary<string, double> { ["2.03"] = -3_846_932.71 };

            Assert.True(IndicadorAlertaPlHelper.DeveOmitirCalculo(
                "Retorno sobre o Patrimônio Líquido (ROE)", valores));
            Assert.Equal("Não Analisar: Informação Comprometida", IndicadorAlertaPlHelper.Mensagem);
        }

        [Fact]
        public void Roe_ComPlZero_TambemOmite()
        {
            var valores = new Dictionary<string, double> { ["2.03"] = 0 };

            Assert.True(IndicadorAlertaPlHelper.DeveOmitirCalculo(
                IndicadorAlertaPlHelper.NomeRoe, valores));
        }

        [Fact]
        public void Roe_ComPlPositivo_Calcula()
        {
            var valores = new Dictionary<string, double> { ["2.03"] = 398_691.87 };

            Assert.False(IndicadorAlertaPlHelper.DeveOmitirCalculo(
                IndicadorAlertaPlHelper.NomeRoe, valores));
        }

        [Theory]
        [InlineData("Capital de Giro de Longo Prazo (CGLP)")]
        [InlineData("Grau de Endividamento")]
        [InlineData("Liquidez Geral")]
        public void DemaisIndicadores_ComPlNegativo_ContinuamCalculando(string nome)
        {
            var valores = new Dictionary<string, double> { ["2.03"] = -3_846_932.71 };

            Assert.False(IndicadorAlertaPlHelper.DeveOmitirCalculo(nome, valores));
        }
    }
}
