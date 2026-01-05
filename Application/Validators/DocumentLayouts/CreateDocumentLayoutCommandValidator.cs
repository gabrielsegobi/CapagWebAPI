using Application.Commands.DocumentLayouts;
using Domain.Contracts.ExtractionRules;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.DocumentLayouts
{
    public class CreateDocumentLayoutCommandValidator : AbstractValidator<CreateDocumentLayoutCommand>
    {
        public CreateDocumentLayoutCommandValidator()
        {
            RuleFor(x => x.CreateDocumentLayoutRequest)
                .NotNull().WithMessage("O corpo da requisição é obrigatório.");

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

            RuleFor(x => x.CreateDocumentLayoutRequest.Active)
                .NotNull()
                .WithMessage("O campo 'Active' é obrigatório.");

            RuleForEach(x => x.CreateDocumentLayoutRequest.ExtractionRules)
                .SetValidator(new ExtractionRuleEntityValidator())
                .When(x => x.CreateDocumentLayoutRequest.ExtractionRules != null);
        }
    }

    public class ExtractionRuleEntityValidator : AbstractValidator<CreateExtractionRuleRequest>
    {
        public ExtractionRuleEntityValidator()
        {
            RuleFor(x => x)
                 .NotNull()
                 .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.LayoutId)
                .NotNull()
                .WithMessage("O campo 'LayoutId' é obrigatório.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("O campo 'LayoutId' é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.FieldLabel)
                .NotEmpty()
                .WithMessage("O campo 'FieldLabel' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'FieldLabel' não pode exceder 50 caracteres.");

            RuleFor(x => x.ExtractionRegex)
                .NotEmpty()
                .WithMessage("O campo 'ExtractionRegex' é obrigatório.")
                .Must(RegexValidatorHelper.IsValidRegex)
                .WithMessage("A expressão regular fornecida é inválida.");

            RuleFor(x => x.RegexGroupIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O índice do grupo da regex deve ser maior ou igual a zero.");

            RuleFor(x => x.DestinationTable)
                .NotEmpty()
                .WithMessage("O campo 'DestinationTable' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");

            RuleFor(x => x.DestinationColumn)
                .NotEmpty()
                .WithMessage("O campo 'DestinationColumn' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationColumn' não pode exceder 64 caracteres.");

            RuleFor(x => x.DataType)
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");
        }
    }
}
