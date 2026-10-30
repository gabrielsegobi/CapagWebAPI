using Application.Helpers;
using FluentAssertions;

namespace UnitTests.Demo
{
    public class CnpjRootHelperTests
    {
        [Theory]
        [InlineData("12.345.678/0001-99", "12345678")]
        [InlineData("12345678000199", "12345678")]
        [InlineData("12345678", "12345678")]
        public void ToRoot_extrai_8_digitos(string input, string expected)
        {
            CnpjRootHelper.ToRoot(input).Should().Be(expected);
        }

        [Fact]
        public void ParseRoots_aceita_lista_separada_por_virgula()
        {
            var roots = CnpjRootHelper.ParseRoots(["12.345.678/0001-99, 87654321, 11111111000191"]);

            roots.Should().BeEquivalentTo(["12345678", "87654321", "11111111"]);
        }

        [Fact]
        public void MatchesRoot_retorna_true_quando_raiz_permitida()
        {
            var allowed = new HashSet<string> { "12345678" };

            CnpjRootHelper.MatchesRoot("12345678000199", allowed).Should().BeTrue();
            CnpjRootHelper.MatchesRoot("99999999000191", allowed).Should().BeFalse();
        }
    }
}
