using Application.Commands.ExtractionRules;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.ExtractionRules
{
    public class CreateExtractionRuleCommandValidator
        : AbstractValidator<CreateExtractionRuleCommand>
    {
        public CreateExtractionRuleCommandValidator()
        {
            RuleFor(x => x.CreateExtractionRuleRequest)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.CreateExtractionRuleRequest.LayoutId)
                .NotNull()
                .WithMessage("O campo 'LayoutId' é obrigatório.")
                .GreaterThanOrEqualTo(0)
                .WithMessage("O campo 'LayoutId' é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.CreateExtractionRuleRequest.FieldLabel)
                .NotEmpty()
                .WithMessage("O campo 'FieldLabel' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'FieldLabel' não pode exceder 50 caracteres.");

            RuleFor(x => x.CreateExtractionRuleRequest.ExtractionRegex)
                .NotEmpty()
                .WithMessage("O campo 'ExtractionRegex' é obrigatório.")
                .Must(RegexValidatorHelper.IsValidRegex)
                .WithMessage("A expressão regular fornecida é inválida.");

            RuleFor(x => x.CreateExtractionRuleRequest.RegexGroupIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O índice do grupo da regex deve ser maior ou igual a zero.");

            RuleFor(x => x.CreateExtractionRuleRequest.DestinationTable)
                .NotEmpty()
                .WithMessage("O campo 'DestinationTable' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");

            RuleFor(x => x.CreateExtractionRuleRequest.DestinationColumn)
                .NotEmpty()
                .WithMessage("O campo 'DestinationColumn' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationColumn' não pode exceder 64 caracteres.");

            RuleFor(x => x.CreateExtractionRuleRequest.DataType)
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");
        }

    }
}
