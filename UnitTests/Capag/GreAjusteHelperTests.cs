using Application.Helpers;
using Domain.Constants;

namespace UnitTests.Capag
{
    public class GreAjusteHelperTests
    {
        [Fact]
        public void AplicarInversao_InverteQuandoMarcado()
        {
            Assert.Equal(-10m, GreAjusteHelper.AplicarInversao(10m, true));
            Assert.Equal(10m, GreAjusteHelper.AplicarInversao(10m, false));
        }

        [Fact]
        public void ResolverValoresManuais_UsaMediaEmTodosOsAnos()
        {
            var valores = new Dictionary<int, decimal>
            {
                [2023] = 10m,
                [2024] = 20m
            };

            var resolvidos = GreAjusteHelper.ResolverValoresManuais(valores, new[] { 2023, 2024, 2025 }, usarMedia: true);

            Assert.Equal(15m, resolvidos[2023]);
            Assert.Equal(15m, resolvidos[2024]);
            Assert.Equal(15m, resolvidos[2025]);
        }

        [Fact]
        public void ParseESerializeValores_RoundTrip()
        {
            var original = new Dictionary<int, decimal> { [2023] = -32_589_007.82m };
            var json = GreAjusteHelper.SerializeValores(original);
            var parsed = GreAjusteHelper.ParseValores(json);

            Assert.Equal(original[2023], parsed[2023]);
        }

        [Theory]
        [InlineData("receita", true)]
        [InlineData("DESPESA", true)]
        [InlineData("outro", false)]
        public void IsTipoValido_reconhece_receita_e_despesa(string tipo, bool expected)
        {
            Assert.Equal(expected, GreAjusteHelper.IsTipoValido(tipo));
        }

        [Fact]
        public void IsAcaoValida_reconhece_acoes_do_prla()
        {
            Assert.True(PrlAConstants.IsAcaoValida("incluir"));
            Assert.True(PrlAConstants.IsAcaoValida("INCLUIR_COM_DESAGIO"));
            Assert.True(PrlAConstants.IsAcaoValida("excluir"));
            Assert.False(PrlAConstants.IsAcaoValida("somar"));
        }
    }
}
