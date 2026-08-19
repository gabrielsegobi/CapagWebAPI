using Application.Commands.CapagSimples;
using Application.Validators.CapagSimples;
using Domain.Contracts.CapagSimples;
using FluentValidation.TestHelper;

namespace UnitTests.CapagSimples
{
    public class UpsertSimplesDeclarationCommandValidatorTests
    {
        private readonly UpsertSimplesDeclarationCommandValidator _validator = new();

        [Fact]
        public void Deve_Falhar_Quando_Simples_Sem_Exercicios()
        {
            var command = new UpsertSimplesDeclarationCommand
            {
                IdEmpresa = 1,
                Request = new UpsertSimplesDeclarationRequest
                {
                    DeclarationKind = "SIMPLES_EXERCISE_YEARS",
                    ExerciseYears = []
                }
            };

            var result = _validator.TestValidate(command);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Deve_Aceitar_NoNational_Com_Lista_Vazia()
        {
            var command = new UpsertSimplesDeclarationCommand
            {
                IdEmpresa = 1,
                Request = new UpsertSimplesDeclarationRequest
                {
                    DeclarationKind = "NO_NATIONAL_SIMPLE_STRICT",
                    ExerciseYears = [2020]
                }
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Aceitar_Simples_Com_Anos_Validos()
        {
            var command = new UpsertSimplesDeclarationCommand
            {
                IdEmpresa = 1,
                Request = new UpsertSimplesDeclarationRequest
                {
                    DeclarationKind = "SIMPLES_EXERCISE_YEARS",
                    ExerciseYears = [2022, 2024]
                }
            };

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
