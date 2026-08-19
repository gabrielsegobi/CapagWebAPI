using Application.Helpers;
using Application.Services.Demonstrativos;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace UnitTests.Capag
{
    public class DemonstrativoArvoreBuilderTests
    {
        [Fact]
        public void RecalcularSomaHierarquica_ExclusaoDeFilhaPropagaParaPai()
        {
            var anos = new[] { 2022 };
            var pai = new LinhaDemonstrativoDto
            {
                Codigo = "3.01.01.03",
                Grupo = GrupoContabil.Despesa,
                Filhos =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03.01",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 100m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03.02",
                        Grupo = GrupoContabil.Despesa,
                        Excluida = true,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 40m } }
                    }
                }
            };

            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(new[] { pai }, anos);

            Assert.Equal(0m, pai.Filhos[1].Valores.Obter(2022));
            Assert.Equal(100m, pai.Valores.Obter(2022));
        }

        [Fact]
        public void RecalcularSomaHierarquica_ReceitaMenosDespesaNosFilhos()
        {
            var anos = new[] { 2022 };
            var receitaLiquida = new LinhaDemonstrativoDto
            {
                Codigo = "3.01.01.01",
                Grupo = GrupoContabil.Receita,
                Filhos =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.01.01",
                        Grupo = GrupoContabil.Receita,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 1000m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.01.02",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = -200m } }
                    }
                }
            };

            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(new[] { receitaLiquida }, anos);

            Assert.Equal(800m, receitaLiquida.Valores.Obter(2022));
        }

        [Fact]
        public void RecalcularSomaHierarquica_NaoSobrescreveResultadoLiquidoDaPropriaLinha()
        {
            var anos = new[] { 2022 };
            var resultado = new LinhaDemonstrativoDto
            {
                Codigo = "3",
                Grupo = GrupoContabil.Receita,
                Valores = new ValoresAnuaisDto { PorAno = { [2022] = 250719.93m } },
                Filhos =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01",
                        Grupo = GrupoContabil.Receita,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 283413.04m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.02.01.01",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 32693.11m } }
                    }
                }
            };

            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(new[] { resultado }, anos);

            Assert.Equal(250719.93m, resultado.Valores.Obter(2022));
        }

        [Fact]
        public void EhCodigoOuFilho_ReconheceDescendentes()
        {
            Assert.True(SaldoContabilHelper.EhCodigoOuFilho("3.01.01.03.01", "3.01.01.03"));
            Assert.True(SaldoContabilHelper.EhCodigoOuFilho("3.01.01.03", "3.01.01.03"));
            Assert.False(SaldoContabilHelper.EhCodigoOuFilho("3.01.01.01", "3.01.01.03"));
        }
    }
}
