using System.Text.Json.Nodes;
using Application.Commands.CapagFco;
using Application.Validators.CapagFco;
using Domain.Contracts.CapagFco;
using FluentValidation.TestHelper;

namespace UnitTests.CapagFco
{
    public class UpsertCapagFcoParametrosCommandValidatorTests
    {
        private readonly UpsertCapagFcoParametrosCommandValidator _validator = new();

        private static UpsertCapagFcoParametrosCommand Command(
            long idEmpresa = 1,
            string bloco = "l100",
            JsonObject? excecoes = null,
            bool excecoesNulas = false,
            string? versaoBase = "a1b2c3d4e5f67890")
        {
            return new UpsertCapagFcoParametrosCommand
            {
                IdEmpresa = idEmpresa,
                Request = new UpsertCapagFcoParametrosRequest
                {
                    Bloco = bloco,
                    Excecoes = excecoesNulas ? null : (excecoes ?? new JsonObject()),
                    VersaoBase = versaoBase
                }
            };
        }

        [Fact]
        public void Deve_Aceitar_Bloco_L100_Com_Excecoes_Vazias()
        {
            var result = _validator.TestValidate(Command());
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Aceitar_L300_Case_Insensitive()
        {
            var result = _validator.TestValidate(Command(bloco: "L300"));
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Falhar_Quando_Bloco_Invalido()
        {
            var result = _validator.TestValidate(Command(bloco: "l200"));
            result.ShouldHaveValidationErrorFor(x => x.Request.Bloco);
        }

        [Fact]
        public void Deve_Falhar_Quando_Excecoes_Nulas()
        {
            var result = _validator.TestValidate(Command(excecoesNulas: true));
            result.ShouldHaveValidationErrorFor(x => x.Request.Excecoes);
        }

        [Fact]
        public void Deve_Falhar_Quando_IdEmpresa_Invalido()
        {
            var result = _validator.TestValidate(Command(idEmpresa: 0));
            result.ShouldHaveValidationErrorFor(x => x.IdEmpresa);
        }

        [Fact]
        public void Deve_Falhar_Quando_VersaoBase_Maior_Que_16()
        {
            var result = _validator.TestValidate(Command(versaoBase: new string('a', 17)));
            result.ShouldHaveValidationErrorFor(x => x.Request.VersaoBase);
        }
    }
}
