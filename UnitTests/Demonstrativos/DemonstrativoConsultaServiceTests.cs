using Application.Services.Demonstrativos;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace UnitTests.Demonstrativos
{
    public class DemonstrativoConsultaServiceTests
    {
        [Fact]
        public void MontarMapaValores_UsaColunasDoBalancoEDaDre_EAberturaComoI()
        {
            var balanco = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "1.01",
                        Grupo = GrupoContabil.Ativo,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 309752.59m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "2.01",
                        Grupo = GrupoContabil.Passivo,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 2628046.94m } }
                    }
                }
            };

            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.01",
                        Grupo = GrupoContabil.Receita,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 7759047.96m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Grupo = GrupoContabil.Receita,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 423309.29m } }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "1.01",
                    Ano = 2022,
                    IsDre = false,
                    SaldoNormalizado = 309752.59m,
                    SaldoInicialNormalizado = 1146164.12m
                },
                new ContaAnoSaldo
                {
                    Codigo = "3",
                    Ano = 2022,
                    IsDre = true,
                    SaldoNormalizado = 250719.93m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(balanco, dre, saldos);

            Assert.Equal(309752.59m, mapa[2022]["1.01"]);
            Assert.Equal(2628046.94m, mapa[2022]["2.01"]);
            Assert.Equal(7759047.96m, mapa[2022]["3.01.01.01"]);
            Assert.Equal(250719.93m, mapa[2022]["3"]);
            Assert.Equal(1146164.12m, mapa[2022]["1.01[I]"]);
        }

        [Fact]
        public void MontarMapaValores_AtivoCreditoFicaNegativoAposInversao()
        {
            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "1.01.01",
                    Ano = 2022,
                    IsDre = false,
                    Indicador = 'C',
                    SaldoNormalizado = 943519.63m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), new DemonstrativoArvoreDto(), saldos);

            Assert.Equal(-943519.63m, mapa[2022]["1.01.01"]);
        }

        [Fact]
        public void MontarMapaValores_Conta3DebitoEntraNegativaNoIndicador()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Grupo = GrupoContabil.Receita,
                        IndicadorDC = IndicadorDC.Debito,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 250719.93m } }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "3",
                    Ano = 2022,
                    IsDre = true,
                    Indicador = 'D',
                    SaldoNormalizado = -250719.93m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, saldos);

            Assert.Equal(-250719.93m, mapa[2022]["3"]);
        }

        [Fact]
        public void MontarMapaValores_Conta3_UsaDcDoPeriodoEmCadaAno()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Grupo = GrupoContabil.Receita,
                        IndicadorDC = IndicadorDC.Debito,
                        Valores = new ValoresAnuaisDto
                        {
                            PorAno =
                            {
                                [2022] = 250719.93m,
                                [2023] = 463906.68m,
                                [2024] = 1392333.27m
                            }
                        }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo { Codigo = "3", Ano = 2022, IsDre = true, Indicador = 'C', SaldoNormalizado = 250719.93m },
                new ContaAnoSaldo { Codigo = "3", Ano = 2023, IsDre = true, Indicador = 'D', SaldoNormalizado = -463906.68m },
                new ContaAnoSaldo { Codigo = "3", Ano = 2024, IsDre = true, Indicador = 'D', SaldoNormalizado = -1392333.27m }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, saldos);

            Assert.Equal(250719.93m, mapa[2022]["3"]);
            Assert.Equal(-463906.68m, mapa[2023]["3"]);
            Assert.Equal(-1392333.27m, mapa[2024]["3"]);
        }

        [Fact]
        public void MontarMapaValores_DreSobrescreveCodigoTambemPresenteNoBalanco()
        {
            var balanco = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 1m } }
                    }
                }
            };
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 67144.61m } }
                    }
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(balanco, dre, Array.Empty<ContaAnoSaldo>());

            Assert.Equal(67144.61m, mapa[2022]["3"]);
        }

        [Fact]
        public void MontarMapaValores_LeituraDaDreVenceArvoreRecalculada()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 0m } }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "3.01.01.03",
                    Ano = 2022,
                    IsDre = true,
                    SaldoNormalizado = 6420822.64m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, saldos);

            Assert.Equal(6420822.64m, mapa[2022]["3.01.01.03"]);
        }

        [Fact]
        public void MontarMapaValores_3010103_DebitoEntraPositivo()
        {
            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "3.01.01.03",
                    Ano = 2022,
                    IsDre = true,
                    Indicador = 'D',
                    SaldoNormalizado = 6420822.64m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), new DemonstrativoArvoreDto(), saldos);

            Assert.Equal(6420822.64m, mapa[2022]["3.01.01.03"]);
        }

        [Fact]
        public void MontarMapaValores_PreenchePaiAusenteComFolhasDaDre()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
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
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 40m } }
                    }
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, Array.Empty<ContaAnoSaldo>());

            Assert.Equal(140m, mapa[2022]["3.01.01.03"]);
        }

        [Fact]
        public void MontarMapaValores_NaoSobrescrevePaiQueJaExiste()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 250719.93m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03.01",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 100m } }
                    }
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, Array.Empty<ContaAnoSaldo>());

            Assert.Equal(250719.93m, mapa[2022]["3.01.01.03"]);
        }

        [Fact]
        public void MontarMapaValores_SubstituiPaiZeradoPelaSomaDasFolhas()
        {
            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 0m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.01.01.03.01",
                        Grupo = GrupoContabil.Despesa,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 6420822.64m } }
                    }
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                new DemonstrativoArvoreDto(), dre, Array.Empty<ContaAnoSaldo>());

            Assert.Equal(6420822.64m, mapa[2022]["3.01.01.03"]);
        }

        [Fact]
        public void MontarMapaValores_AberturaZeradaNaoEhPreenchidaComFilhos()
        {
            var balanco = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "2.01.01.03",
                        Grupo = GrupoContabil.Passivo,
                        Valores = new ValoresAnuaisDto { PorAno = { [2022] = 1186609.02m } }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "2.01.01.03",
                    Ano = 2022,
                    IsDre = false,
                    SaldoNormalizado = 1186609.02m,
                    SaldoInicialNormalizado = 0m
                },
                new ContaAnoSaldo
                {
                    Codigo = "2.01.01.03.01",
                    Ano = 2022,
                    IsDre = false,
                    SaldoNormalizado = 100m,
                    SaldoInicialNormalizado = 50m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(
                balanco, new DemonstrativoArvoreDto(), saldos);

            Assert.Equal(0m, mapa[2022]["2.01.01.03[I]"]);
            Assert.Equal(50m, mapa[2022]["2.01.01.03.01[I]"]);
        }

        [Fact]
        public void MontarMapaValores_PlDoBalancoComo3_FicaDisponivelComo203NasFormulas()
        {
            var balanco = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Grupo = GrupoContabil.PatrimonioLiquido,
                        Valores = new ValoresAnuaisDto { PorAno = { [2023] = 3846932.71m } }
                    },
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3.04",
                        Grupo = GrupoContabil.PatrimonioLiquido,
                        Valores = new ValoresAnuaisDto { PorAno = { [2023] = 3866932.71m } }
                    }
                }
            };

            var dre = new DemonstrativoArvoreDto
            {
                Linhas =
                {
                    new LinhaDemonstrativoDto
                    {
                        Codigo = "3",
                        Grupo = GrupoContabil.Receita,
                        IndicadorDC = IndicadorDC.Debito,
                        Valores = new ValoresAnuaisDto { PorAno = { [2023] = 1000m } }
                    }
                }
            };

            var saldos = new[]
            {
                new ContaAnoSaldo
                {
                    Codigo = "3",
                    Ano = 2023,
                    IsDre = false,
                    Indicador = 'D',
                    IndicadorInicial = 'C',
                    SaldoNormalizado = 3846932.71m,
                    SaldoInicialNormalizado = 100m
                },
                new ContaAnoSaldo
                {
                    Codigo = "3",
                    Ano = 2023,
                    IsDre = true,
                    Indicador = 'D',
                    SaldoNormalizado = -1000m
                },
                new ContaAnoSaldo
                {
                    Codigo = "3.04",
                    Ano = 2023,
                    IsDre = false,
                    Indicador = 'D',
                    SaldoNormalizado = 3866932.71m
                }
            };

            var mapa = DemonstrativoConsultaService.MontarMapaValores(balanco, dre, saldos);

            Assert.Equal(-3846932.71m, mapa[2023]["2.03"]);
            Assert.Equal(-3866932.71m, mapa[2023]["2.03.04"]);
            Assert.Equal(100m, mapa[2023]["2.03[I]"]);
            Assert.Equal(-1000m, mapa[2023]["3"]);
        }
    }
}
