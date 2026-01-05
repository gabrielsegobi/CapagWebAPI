using Application.Commands.ExtractionRules;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.ExtractionRules
{
    public class UpdateExtractionRuleCommandValidator : AbstractValidator<UpdateExtractionRuleCommand>
    {
        public UpdateExtractionRuleCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID da regra é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.UpdateExtractionRuleRequest)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.UpdateExtractionRuleRequest.FieldLabel)
                .NotEmpty()
                .WithMessage("O campo 'FieldLabel' é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo 'FieldLabel' não pode exceder 50 caracteres.");

            RuleFor(x => x.UpdateExtractionRuleRequest.ExtractionRegex)
                .NotEmpty()
                .WithMessage("O campo 'ExtractionRegex' é obrigatório.")
                .Must(RegexValidatorHelper.IsValidRegex)
                .WithMessage("A expressão regular fornecida é inválida.");

            RuleFor(x => x.UpdateExtractionRuleRequest.RegexGroupIndex)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O índice do grupo da regex deve ser maior ou igual a zero.");

            RuleFor(x => x.UpdateExtractionRuleRequest.DestinationTable)
                .NotEmpty()
                .WithMessage("O campo 'DestinationTable' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");

            RuleFor(x => x.UpdateExtractionRuleRequest.DestinationColumn)
                .NotEmpty()
                .WithMessage("O campo 'DestinationColumn' é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationColumn' não pode exceder 64 caracteres.");

            RuleFor(x => x.UpdateExtractionRuleRequest.DataType)
                .MaximumLength(64)
                .WithMessage("O campo 'DestinationTable' não pode exceder 64 caracteres.");
        }
    }
}
