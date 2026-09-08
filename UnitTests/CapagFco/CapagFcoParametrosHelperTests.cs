using System.Text.Json.Nodes;
using Application.Helpers;
using Domain.Entities;
using FluentAssertions;

namespace UnitTests.CapagFco
{
    public class CapagFcoParametrosHelperTests
    {
        [Theory]
        [InlineData("l100", true)]
        [InlineData("L100", true)]
        [InlineData("l300", true)]
        [InlineData("L300", true)]
        [InlineData(" l100 ", true)]
        [InlineData("l200", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IsValidBloco_reconhece_l100_e_l300(string? bloco, bool expected)
        {
            CapagFcoParametrosHelper.IsValidBloco(bloco).Should().Be(expected);
        }

        [Fact]
        public void ParseExcecoes_json_vazio_ou_invalido_retorna_objeto_vazio()
        {
            CapagFcoParametrosHelper.ParseExcecoes(null).Count.Should().Be(0);
            CapagFcoParametrosHelper.ParseExcecoes("").Count.Should().Be(0);
            CapagFcoParametrosHelper.ParseExcecoes("{}").Count.Should().Be(0);
            CapagFcoParametrosHelper.ParseExcecoes("[]").Count.Should().Be(0);
            CapagFcoParametrosHelper.ParseExcecoes("nao-json").Count.Should().Be(0);
        }

        [Fact]
        public void ParseExcecoes_preserva_campos_do_delta()
        {
            var parsed = CapagFcoParametrosHelper.ParseExcecoes(
                """{"1.02.03.01.30":{"grupoDfc":"SEM EFEITO DE CAIXA","trat":"T9"}}""");

            parsed.Count.Should().Be(1);
            parsed["1.02.03.01.30"]!["trat"]!.GetValue<string>().Should().Be("T9");
            parsed["1.02.03.01.30"]!["grupoDfc"]!.GetValue<string>().Should().Be("SEM EFEITO DE CAIXA");
        }

        [Fact]
        public void NormalizeExcecoes_trim_chave_e_ignora_vazias()
        {
            var source = new JsonObject
            {
                [" 1.02.03.01.30 "] = new JsonObject { ["trat"] = "T9" },
                [" "] = new JsonObject { ["trat"] = "T1" },
                ["1.01"] = null
            };

            var normalized = CapagFcoParametrosHelper.NormalizeExcecoes(source);

            normalized.Count.Should().Be(1);
            normalized.ContainsKey("1.02.03.01.30").Should().BeTrue();
        }

        [Fact]
        public void SerializeExcecoes_objeto_vazio_vira_json_vazio()
        {
            CapagFcoParametrosHelper.SerializeExcecoes(null).Should().Be("{}");
            CapagFcoParametrosHelper.SerializeExcecoes(new JsonObject()).Should().Be("{}");
        }

        [Fact]
        public void ToResponse_sem_entidade_retorna_default_vazio()
        {
            var response = CapagFcoParametrosHelper.ToResponse(123, null);

            response.CompanyId.Should().Be(123);
            response.ExcecoesL100.Count.Should().Be(0);
            response.ExcecoesL300.Count.Should().Be(0);
            response.BaseVersaoHash.Should().BeNull();
            response.UpdatedAt.Should().BeNull();
            response.ContasPersonalizadasL100.Should().Be(0);
            response.ContasPersonalizadasL300.Should().Be(0);
        }

        [Fact]
        public void ToResponse_conta_chaves_de_cada_bloco()
        {
            var entity = new CapagFcoParametroEmpresa
            {
                ExcecoesL100Json = """{"1.01":{"trat":"T9"},"1.02":{"acao":"X"}}""",
                ExcecoesL300Json = "{}",
                BaseVersaoHash = "a1b2c3d4e5f67890",
                UpdatedAt = new DateTime(2026, 8, 26, 21, 0, 0)
            };

            var response = CapagFcoParametrosHelper.ToResponse(123, entity);

            response.ContasPersonalizadasL100.Should().Be(2);
            response.ContasPersonalizadasL300.Should().Be(0);
            response.BaseVersaoHash.Should().Be("a1b2c3d4e5f67890");
        }

        [Fact]
        public void NormalizeVersaoBase_trim_e_vazio_vira_null()
        {
            CapagFcoParametrosHelper.NormalizeVersaoBase(null).Should().BeNull();
            CapagFcoParametrosHelper.NormalizeVersaoBase("  ").Should().BeNull();
            CapagFcoParametrosHelper.NormalizeVersaoBase("  abc  ").Should().Be("abc");
        }
    }
}
