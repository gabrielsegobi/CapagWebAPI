using Application.Commands.PrlA;
using FluentValidation;

namespace Application.Validators.PrlA
{
    public class PatchSaldoPrlACommandValidator : AbstractValidator<PatchSaldoPrlACommand>
    {
        public PatchSaldoPrlACommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.CodigoConta)
                .NotEmpty()
                .WithMessage("O código da conta é obrigatório.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");
        }
    }
}
