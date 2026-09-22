using Application.Commands.Capag;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.Capag
{
    public class CreateContaGreManualCommandValidator : AbstractValidator<CreateContaGreManualCommand>
    {
        public CreateContaGreManualCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.Descricao)
                .NotEmpty()
                .WithMessage("A descrição da conta manual é obrigatória.")
                .MaximumLength(255)
                .WithMessage("A descrição deve ter no máximo 255 caracteres.");

            RuleFor(x => x.Request.Tipo)
                .Must(GreAjusteHelper.IsTipoValido)
                .WithMessage("O tipo deve ser receita ou despesa.");

            RuleFor(x => x.Request.Justificativa)
                .MaximumLength(500)
                .When(x => x.Request?.Justificativa != null)
                .WithMessage("A justificativa deve ter no máximo 500 caracteres.");

            RuleFor(x => x.Request.CodigoConta)
                .MaximumLength(64)
                .When(x => x.Request?.CodigoConta != null)
                .WithMessage("O código da conta deve ter no máximo 64 caracteres.");
        }
    }
}
