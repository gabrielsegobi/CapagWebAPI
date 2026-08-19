using Application.Helpers;
using Domain.Constants;

namespace UnitTests.Helpers
{
    public class PatrimonioLiquidoCodigoMapperTests
    {
        [Theory]
        [InlineData("3", "2.03")]
        [InlineData("3.01", "2.03.01")]
        [InlineData("3.04", "2.03.04")]
        [InlineData("3.04[I]", "2.03.04[I]")]
        [InlineData("3[I]", "2.03[I]")]
        public void ParaCodigoFormula_MapeiaContaDoBalanco(string balanco, string formula)
        {
            Assert.Equal(formula, PatrimonioLiquidoCodigoMapper.ParaCodigoFormula(balanco));
        }

        [Fact]
        public void EhCodigoBalanco_NaoConfundeComCosif203()
        {
            Assert.True(PatrimonioLiquidoCodigoMapper.EhCodigoBalanco("3"));
            Assert.True(PatrimonioLiquidoCodigoMapper.EhCodigoBalanco("3.04"));
            Assert.False(PatrimonioLiquidoCodigoMapper.EhCodigoBalanco("2.03"));
            Assert.False(PatrimonioLiquidoCodigoMapper.EhCodigoBalanco("2.03.01"));
            Assert.True(PatrimonioLiquidoCodigoMapper.EhCodigoFormula("2.03"));
            Assert.Equal(PatrimonioLiquidoConstants.CodigoFormula, "2.03");
        }
    }
}
