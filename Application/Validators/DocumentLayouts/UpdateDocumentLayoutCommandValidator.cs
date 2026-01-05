using Application.Commands.DocumentLayouts;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.DocumentLayouts
{
    public class UpdateDocumentLayoutCommandValidator : AbstractValidator<UpdateDocumentLayoutCommand>
    {
        public UpdateDocumentLayoutCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O campo 'Id' deve ser maior que zero.");

            RuleFor(x => x.UpdateDocumentLayoutRequest)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.UpdateDocumentLayoutRequest.LayoutName)
                .NotEmpty()
                .WithMessage("O campo 'LayoutName' é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O campo 'LayoutName' não pode exceder 100 caracteres.");

            RuleFor(x => x.UpdateDocumentLayoutRequest.Description)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.UpdateDocumentLayoutRequest.Description))
                .WithMessage("O campo 'Description' não pode exceder 255 caracteres.");

            RuleFor(x => x.UpdateDocumentLayoutRequest.ValidationRegex)
                .NotEmpty()
                .WithMessage("O campo 'ValidationRegex' é obrigatório.")
                .Must(RegexValidatorHelper.IsValidRegex)
                .WithMessage("A expressão regular fornecida em 'ValidationRegex' é inválida.");

            RuleFor(x => x.UpdateDocumentLayoutRequest.Active)
                .NotNull()
                .WithMessage("O campo 'Active' é obrigatório.");
        }
    }
}
