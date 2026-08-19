using Application.Services.Capag;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace UnitTests.Capag
{
    public class LucroBrutoCalculatorTests
    {
        [Fact]
        public void Calcular_ReceitaLiquidaMenosGrupoCustos_InsereApos3010103()
        {
            var anos = new[] { 2022, 2023, 2024 };
            var dre = new List<LinhaDemonstrativoDto>
            {
                new()
                {
                    Codigo = "3.01.01",
                    Descricao = "RESULTADO OPERACIONAL",
                    Grupo = GrupoContabil.Receita,
                    Filhos =
                    {
                        new LinhaDemonstrativoDto
                        {
                            Codigo = DreCapagConstants.CodigoReceitaLiquida,
                            Descricao = "RECEITA LÍQUIDA",
                            Grupo = GrupoContabil.Receita,
                            Valores = new ValoresAnuaisDto
                            {
                                PorAno = { [2022] = 1000m, [2023] = 1100m, [2024] = 1200m }
                            }
                        },
                        new LinhaDemonstrativoDto
                        {
                            Codigo = DreCapagConstants.CodigoGrupoCustos,
                            Descricao = "CUSTO DOS BENS E SERVIÇOS",
                            Grupo = GrupoContabil.Despesa,
                            Valores = new ValoresAnuaisDto
                            {
                                PorAno = { [2022] = -400m, [2023] = -450m, [2024] = -500m }
                            },
                            Filhos =
                            {
                                new LinhaDemonstrativoDto
                                {
                                    Codigo = "3.01.01.03.01",
                                    Descricao = "CMV",
                                    Grupo = GrupoContabil.Despesa,
                                    Valores = new ValoresAnuaisDto
                                    {
                                        PorAno = { [2022] = -400m, [2023] = -450m, [2024] = -500m }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var calculator = new LucroBrutoCalculator();
            calculator.InserirNaArvore(dre, anos);

            var lucro = dre[0].Filhos.Single(f => f.Codigo == DreCapagConstants.CodigoLucroBrutoCalculado);

            Assert.True(lucro.IsLinhaCalculada);
            Assert.Equal("LUCRO BRUTO", lucro.Descricao);
            Assert.Equal(600m, lucro.Valores.Obter(2022));
            Assert.Equal(650m, lucro.Valores.Obter(2023));
            Assert.Equal(700m, lucro.Valores.Obter(2024));

            var idxCustos = dre[0].Filhos.FindIndex(f => f.Codigo == DreCapagConstants.CodigoGrupoCustos);
            var idxLucro = dre[0].Filhos.FindIndex(f => f.Codigo == DreCapagConstants.CodigoLucroBrutoCalculado);
            Assert.Equal(idxCustos + 1, idxLucro);
            Assert.True(lucro.Posicao > idxCustos);
        }
    }
}
