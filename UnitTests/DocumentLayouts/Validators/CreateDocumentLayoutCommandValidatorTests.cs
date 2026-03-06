using Application.Commands.DocumentLayouts;
using Application.Validators.DocumentLayouts;
using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.ValidatioRegexes;
using FluentValidation.TestHelper;
using TestSupport.Fakes.DocumentLayouts;

namespace UnitTests.DocumentLayouts.Validators
{
    public class CreateDocumentLayoutCommandValidatorTests
    {
        private readonly CreateDocumentLayoutCommandValidator _validator;

        public CreateDocumentLayoutCommandValidatorTests()
        {
            _validator = new CreateDocumentLayoutCommandValidator();
        }

        private CreateDocumentLayoutCommand CreateValidCommand()
        {
            var request = new CreateDocumentLayoutRequestFaker()
                .WithValidationRegex()
                .Generate();

            return new CreateDocumentLayoutCommand(request);
        }

        // ===============================
        // ✅ CENÁRIO FELIZ
        // ===============================

        [Fact]
        public void Deve_Passar_Quando_Comando_For_Valido()
        {
            var command = CreateValidCommand();

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        // ===============================
        // 📌 REQUEST NULL
        // ===============================

        [Fact]
        public void Deve_Falhar_Quando_Request_For_Null()
        {
            var command = new CreateDocumentLayoutCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest);
        }

        // ===============================
        // 📌 LayoutName
        // ===============================

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Deve_Falhar_Quando_LayoutName_For_Invalido(string layoutName)
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.LayoutName = layoutName;

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.LayoutName);
        }

        [Fact]
        public void Deve_Falhar_Quando_LayoutName_Exceder_100_Caracteres()
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.LayoutName = new string('A', 101);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.LayoutName);
        }

        // ===============================
        // 📌 Description
        // ===============================

        [Fact]
        public void Deve_Falhar_Quando_Description_Exceder_255_Caracteres()
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.Description = new string('A', 256);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.Description);
        }

        // ===============================
        // 📌 ValidationRegex
        // ===============================

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Deve_Falhar_Quando_ValidationRegex_For_Vazio(string regex)
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.ValidationRegex = regex;

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.ValidationRegex);
        }

        [Fact]
        public void Deve_Falhar_Quando_ValidationRegex_For_Invalido()
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.ValidationRegex = "[";

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.ValidationRegex);
        }

        // ===============================
        // 📌 System
        // ===============================

        [Fact]
        public void Deve_Falhar_Quando_System_For_Null()
        {
            var command = CreateValidCommand();
            command.CreateDocumentLayoutRequest.System = null;

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CreateDocumentLayoutRequest.System);
        }
    }
    //public class CreateDocumentLayoutCommandValidatorTests
    //{
    //    private readonly CreateDocumentLayoutCommandValidator _validator;

    //    public CreateDocumentLayoutCommandValidatorTests()
    //    {
    //        _validator = new CreateDocumentLayoutCommandValidator();
    //    }

    //    [Theory]
    //    // layoutName, validationRegex, system, regexInterna, esperadoValido
    //    [InlineData("Layout", "Layout", "\\d+", true, "\\d+", true)]      // tudo válido
    //    [InlineData("Layout", "Layout", "\\d+", false, "", false)]         // regex interna vazia
    //    [InlineData("Layout", "Layout", "\\d+", false, "[", false)]        // regex interna inválida
    //    [InlineData("Layout", "Layout", "\\d+", false, null, false)]       // regex interna null
    //    public void Deve_Validar_ValidationRegexes(
    //        string layoutName,
    //        string description,
    //        string validationRegex,
    //        bool system,
    //        string regexInterna,
    //        bool esperadoValido)
    //    {
    //        var request = new CreateDocumentLayoutRequest
    //        {
    //            LayoutName = layoutName,
    //            Description = description,
    //            ValidationRegex = validationRegex,
    //            System = system,
    //            ValidationRegexes = new List<CreateValidationRegexRequest>
    //            {
    //                new CreateValidationRegexRequest
    //                {
    //                    Regex = regexInterna
    //                }
    //            }
    //        };


    //        var command = new CreateDocumentLayoutCommand(request);

    //        var result = _validator.TestValidate(command);

    //        Assert.Equal(esperadoValido, result.IsValid);
    //    }
    //}
}
