using Application.Commands.DocumentLayouts;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.ValidatioRegexes;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.DocumentLayouts
{
    public class CreateDocumentLayoutCommandValidator : AbstractValidator<CreateDocumentLayoutCommand>
    {
        public CreateDocumentLayoutCommandValidator()
        {
            RuleFor(x => x.CreateDocumentLayoutRequest)
                .NotNull().WithMessage("O corpo da requisição é obrigatório.")
                .DependentRules(() =>
                {

                    RuleFor(x => x.CreateDocumentLayoutRequest.LayoutName)
                .NotEmpty().WithMessage("O campo 'LayoutName' é obrigatório.")
                .MaximumLength(100).WithMessage("O campo 'LayoutName' não pode exceder 100 caracteres.");

                    RuleFor(x => x.CreateDocumentLayoutRequest.Description)
                        .MaximumLength(255)
                        .When(x => !string.IsNullOrWhiteSpace(x.CreateDocumentLayoutRequest.Description))
                        .WithMessage("O campo 'Description' não pode exceder 255 caracteres.");

                    RuleFor(x => x.CreateDocumentLayoutRequest.ValidationRegex)
                        .NotEmpty()
                        .WithMessage("O campo 'ValidationRegex' é obrigatório.")
                        .Must(RegexValidatorHelper.IsValidRegex)
                        .WithMessage("A expressão regular fornecida em 'ValidationRegex' é inválida.");

                    RuleFor(x => x.CreateDocumentLayoutRequest.System)
                        .NotNull()
                        .WithMessage("O campo 'System' é obrigatório.");

                    RuleForEach(x => x.CreateDocumentLayoutRequest.ValidationRegexes)
                        .SetValidator(new ExtractionRuleEntityValidator())
                        .When(x => x.CreateDocumentLayoutRequest.ValidationRegexes != null);
                });
        }
    }

    public class ExtractionRuleEntityValidator : AbstractValidator<CreateValidationRegexRequest>
    {
        public ExtractionRuleEntityValidator()
        {
            RuleFor(x => x)
                 .NotNull()
                 .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Regex)
                .NotEmpty()
                .WithMessage("O campo 'regex' é obrigatório.")
                .Must(RegexValidatorHelper.IsValidRegex)
                .WithMessage("A expressão regular fornecida é inválida.");
        }
    }
}
