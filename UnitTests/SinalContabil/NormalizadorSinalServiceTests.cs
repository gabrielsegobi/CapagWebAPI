using Application.Services.SinalContabil;
using Domain.Enums;
using Domain.Models;

namespace UnitTests.SinalContabil
{
    public class NormalizadorSinalServiceTests
    {
        private readonly NormalizadorSinalService _sut = new(new GrupoContabilResolver());

        [Fact]
        public void Credito_SemprePositivo_Debito_SempreNegativo()
        {
            Assert.Equal(-943_519.63m, _sut.Normalizar("1.01.01", 943_519.63m, 'C'));
            Assert.Equal(141_232.12m, _sut.Normalizar("1.01.01", 141_232.12m, 'D'));
            Assert.Equal(309_752.59m, _sut.Normalizar("1.01", 309_752.59m, 'D'));
            Assert.Equal(2_628_046.94m, _sut.Normalizar("2.01", 2_628_046.94m, 'C'));
            Assert.Equal(-40m, _sut.Normalizar("2.01.01.03", 40m, 'D'));
            Assert.Equal(-1_049_174.32m, _sut.Normalizar("2.03", 1_049_174.32m, 'D'));
            Assert.Equal(398_691.87m, _sut.Normalizar("2.03", 398_691.87m, 'C'));
            Assert.Equal(-80m, _sut.Normalizar("3.01.01", 80m, 'D'));
            Assert.Equal(80m, _sut.Normalizar("3.01.01", 80m, 'C'));
            Assert.Equal(12.5m, _sut.Normalizar("3.01.01.03", -12.5m, 'C'));
            Assert.Equal(12.5m, _sut.Normalizar("3.01.01.03", 12.5m, 'D'));
        }

        [Theory]
        [InlineData(GrupoContabil.Ativo, IndicadorDC.Debito, 100, -100)]
        [InlineData(GrupoContabil.Ativo, IndicadorDC.Credito, 100, 100)]
        [InlineData(GrupoContabil.Passivo, IndicadorDC.Credito, 50, 50)]
        [InlineData(GrupoContabil.Passivo, IndicadorDC.Debito, 50, -50)]
        [InlineData(GrupoContabil.PatrimonioLiquido, IndicadorDC.Credito, 80, 80)]
        [InlineData(GrupoContabil.PatrimonioLiquido, IndicadorDC.Debito, 80, -80)]
        [InlineData(GrupoContabil.Receita, IndicadorDC.Credito, 200, 200)]
        [InlineData(GrupoContabil.Receita, IndicadorDC.Debito, 200, -200)]
        [InlineData(GrupoContabil.Despesa, IndicadorDC.Debito, 30, -30)]
        [InlineData(GrupoContabil.Despesa, IndicadorDC.Credito, 30, 30)]
        public void Normalizar_IgnoraNaturezaDoGrupo(
            GrupoContabil grupo,
            IndicadorDC indicador,
            decimal bruto,
            decimal esperado)
        {
            var conta = new ContaContabil
            {
                Codigo = "x",
                Grupo = grupo,
                IndicadorDC = indicador,
                SaldoBruto = bruto
            };

            Assert.Equal(esperado, _sut.Normalizar(conta));
        }

        [Fact]
        public void Lucros_Balanco2022_Credito_DeveSerPositivo()
        {
            var conta = new ContaContabil
            {
                Codigo = "2.03.01",
                Descricao = "Lucros",
                Grupo = GrupoContabil.PatrimonioLiquido,
                IndicadorDC = IndicadorDC.Credito,
                SaldoBruto = 185_432.10m
            };

            Assert.Equal(185_432.10m, _sut.Normalizar(conta));
        }

        [Fact]
        public void Normalizar_3010103_IgnoraDcEFicaPositivo()
        {
            Assert.Equal(-80m, _sut.Normalizar("3.01.01", 80m, 'D'));
            Assert.Equal(80m, _sut.Normalizar("3.01.01", 80m, 'C'));
            Assert.Equal(12.5m, _sut.Normalizar("3.01.01.03", 12.5m, 'D'));
            Assert.Equal(12.5m, _sut.Normalizar("3.01.01.03", -12.5m, 'C'));
            Assert.Equal(-40m, _sut.Normalizar("3.01.01.03.01", 40m, 'D'));
        }

        [Fact]
        public void Normalizar_SemIndicador_PermaneceAbsoluto()
        {
            Assert.Equal(943_519.63m, _sut.Normalizar("1.01.01", -943_519.63m, null));
        }
    }
}
