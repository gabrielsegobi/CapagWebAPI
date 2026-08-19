using Application.Services.Capag;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace UnitTests.SinalContabil
{
    public class ResultadoLiquidoCalculatorTests
    {
        private readonly ResultadoLiquidoCalculator _sut = new();

        [Fact]
        public void ResultadoLiquido_UsaContaPropriaNormalizada_NaoSomaCruasDosFilhos()
        {
            var anos = new[] { 2022 };
            var dre = new List<LinhaDemonstrativoDto>
            {
                new()
                {
                    Codigo = DreCapagConstants.CodigoResultadoLiquido,
                    Descricao = "RESULTADO LÍQUIDO DO PERÍODO",
                    Grupo = GrupoContabil.Receita,
                    IndicadorDC = IndicadorDC.Credito,
                    Valores = new ValoresAnuaisDto { PorAno = { [2022] = 67_144.61m } },
                    Filhos =
                    {
                        new LinhaDemonstrativoDto
                        {
                            Codigo = "3.01.01.01",
                            Grupo = GrupoContabil.Receita,
                            Valores = new ValoresAnuaisDto { PorAno = { [2022] = 180_000.00m } }
                        },
                        new LinhaDemonstrativoDto
                        {
                            Codigo = "3.01.01.03",
                            Grupo = GrupoContabil.Despesa,
                            Valores = new ValoresAnuaisDto { PorAno = { [2022] = 70_719.93m } }
                        }
                    }
                }
            };

            var resultado = _sut.Calcular(dre, anos);

            Assert.Equal(67_144.61m, resultado.Obter(2022));
            Assert.NotEqual(250_719.93m, resultado.Obter(2022));
            Assert.NotEqual(180_000.00m + 70_719.93m, resultado.Obter(2022));
        }
    }
}
