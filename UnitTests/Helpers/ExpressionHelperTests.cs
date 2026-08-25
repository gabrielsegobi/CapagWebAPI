using Application.Helpers;

namespace UnitTests.Helpers
{
    public class ExpressionHelperTests
    {
        [Fact]
        public void TemReferenciaIndicador_DetectaSintaxeArroba()
        {
            Assert.True(ExpressionHelper.TemReferenciaIndicador("{@PME} + {@PMR} - {@PMP}"));
            Assert.False(ExpressionHelper.TemReferenciaIndicador("{1.01.03} / {2.01}"));
            Assert.False(ExpressionHelper.TemReferenciaIndicador(null));
        }

        [Fact]
        public void SubstituirCodigos_NaoConsomePlaceholderDeIndicador()
        {
            var formula = "{@PME} + {1.01}";
            var expressao = ExpressionHelper.SubstituirCodigos(formula, new Dictionary<string, double>
            {
                ["1.01"] = 10
            });

            Assert.Equal("{@PME} + 10", expressao);
        }

        [Fact]
        public void SubstituirReferenciasIndicadores_UsaResultadosDoAno()
        {
            var formula = "{@PME} + {@PMR} - {@PMP}";
            var resultados = new Dictionary<string, double?>(StringComparer.OrdinalIgnoreCase)
            {
                [SaldoContabilHelper.TagPme] = 0,
                [SaldoContabilHelper.TagPmr] = 20.532971,
                [SaldoContabilHelper.TagPmp] = 48.854088
            };

            var expressao = ExpressionHelper.SubstituirReferenciasIndicadores(formula, resultados);
            Assert.Equal("0 + 20.532971 - 48.854088", expressao);
            Assert.Equal(-28.321117, Math.Round(ExpressionHelper.AvaliarExpressao(expressao), 6));
        }

        [Fact]
        public void SubstituirReferenciasIndicadores_NuloOuAusenteViraZero()
        {
            var formula = "{@A} + {@B}";
            var resultados = new Dictionary<string, double?>
            {
                ["A"] = null
            };

            Assert.Equal("0 + 0", ExpressionHelper.SubstituirReferenciasIndicadores(formula, resultados));
        }

        [Fact]
        public void RegistrarResultadoIndicador_IndexaSomentePorTag()
        {
            var map = new Dictionary<string, double?>(StringComparer.OrdinalIgnoreCase);
            var formula = new Domain.Contracts.Json.FormulaJson
            {
                Nome = SaldoContabilHelper.NomePmp,
                Tag = SaldoContabilHelper.TagPmp
            };

            SaldoContabilHelper.RegistrarResultadoIndicador(map, formula, 45.887954);

            Assert.Equal(45.887954, map[SaldoContabilHelper.TagPmp]);
            Assert.False(map.ContainsKey(SaldoContabilHelper.NomePmp));
        }

        [Fact]
        public void RegistrarResultadoIndicador_SemTag_NaoIndexa()
        {
            var map = new Dictionary<string, double?>(StringComparer.OrdinalIgnoreCase);
            SaldoContabilHelper.RegistrarResultadoIndicador(
                map,
                new Domain.Contracts.Json.FormulaJson { Nome = "Liquidez Geral" },
                1.5);

            Assert.Empty(map);
        }

        [Fact]
        public void CicloFinanceiro_CompoePmePmrPmp_Caso2022FornecedorNegativo()
        {
            // Estoque zerado → PME/PMR = 0; PMP com magnitude no fornecedor inicial negativo.
            const string formulaPmp =
                "((({2.01.01.03[I]} + {2.01.01.03}) / 2) * 365) / ({1.01.03} + {3.01.01.03} - {1.01.03[I]})";
            const string formulaCiclo = "{@PME} + {@PMR} - {@PMP}";

            var valores = new Dictionary<string, double>
            {
                ["3.01.01.03"] = 6420822.64,
                ["1.01.03"] = 0,
                ["1.01.03[I]"] = 0,
                ["1.01.02.02"] = 0,
                ["1.01.02.02[I]"] = 0,
                ["3.01.01.01.01"] = 8829980.74,
                ["2.01.01.03"] = 1186609.02,
                ["2.01.01.03[I]"] = -427848.06
            };

            var pmp = Math.Round(
                ExpressionHelper.ProcessarFormula(
                    formulaPmp,
                    SaldoContabilHelper.ValoresParaFormula(valores, SaldoContabilHelper.NomePmp)),
                6);

            var resultadosAno = new Dictionary<string, double?>(StringComparer.OrdinalIgnoreCase);
            SaldoContabilHelper.RegistrarResultadoIndicador(
                resultadosAno,
                new Domain.Contracts.Json.FormulaJson
                {
                    Nome = SaldoContabilHelper.NomePme,
                    Tag = SaldoContabilHelper.TagPme
                },
                0);
            SaldoContabilHelper.RegistrarResultadoIndicador(
                resultadosAno,
                new Domain.Contracts.Json.FormulaJson
                {
                    Nome = SaldoContabilHelper.NomePmr,
                    Tag = SaldoContabilHelper.TagPmr
                },
                0);
            SaldoContabilHelper.RegistrarResultadoIndicador(
                resultadosAno,
                new Domain.Contracts.Json.FormulaJson
                {
                    Nome = SaldoContabilHelper.NomePmp,
                    Tag = SaldoContabilHelper.TagPmp
                },
                pmp);

            var ciclo = Math.Round(
                ExpressionHelper.ProcessarFormula(
                    formulaCiclo,
                    new Dictionary<string, double>(),
                    resultadosAno),
                6);

            Assert.Equal(45.887954, pmp);
            Assert.Equal(Math.Round(-pmp, 6), ciclo);
        }
    }
}
