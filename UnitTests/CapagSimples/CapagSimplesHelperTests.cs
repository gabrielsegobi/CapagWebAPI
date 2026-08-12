using Application.Helpers;

namespace UnitTests.CapagSimples
{
    public class CapagSimplesHelperTests
    {
        [Fact]
        public void NormalizeExerciseYears_Filtra_Distinct_E_Ordena()
        {
            var result = CapagSimplesHelper.NormalizeExerciseYears([2024, 1900, 2100, 2022, 2024, 1899]);

            Assert.Equal([2022, 2024], result);
        }

        [Fact]
        public void FlattenPorCodigo_Ignora_Codigo_Em_Branco_E_Ano_Invalido()
        {
            var porCodigo = new Dictionary<string, Dictionary<string, decimal>>
            {
                ["  3.01.01  "] = new() { ["2024"] = 10m, ["1800"] = 1m },
                ["   "] = new() { ["2023"] = 5m },
                ["1.01"] = new() { ["abc"] = 9m }
            };

            var rows = CapagSimplesHelper.FlattenPorCodigo(porCodigo);

            Assert.Single(rows);
            Assert.Equal("3.01.01", rows[0].AccountCode);
            Assert.Equal(2024, rows[0].Year);
            Assert.Equal(10m, rows[0].Value);
        }

        [Theory]
        [InlineData("DRE", true)]
        [InlineData("BALANCE_SHEET", true)]
        [InlineData("dre", false)]
        [InlineData("INVALID", false)]
        public void IsValidDemonstrativeKind(string kind, bool expected)
        {
            Assert.Equal(expected, CapagSimplesHelper.IsValidDemonstrativeKind(kind));
        }

        [Theory]
        [InlineData("SIMPLES_EXERCISE_YEARS", true)]
        [InlineData("NO_NATIONAL_SIMPLE_STRICT", true)]
        [InlineData("OTHER", false)]
        public void IsValidDeclarationKind(string kind, bool expected)
        {
            Assert.Equal(expected, CapagSimplesHelper.IsValidDeclarationKind(kind));
        }
    }
}
