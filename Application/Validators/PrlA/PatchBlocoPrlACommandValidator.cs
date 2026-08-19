using Application.Commands.PrlA;
using FluentValidation;

namespace Application.Validators.PrlA
{
    public class PatchBlocoPrlACommandValidator : AbstractValidator<PatchBlocoPrlACommand>
    {
        public PatchBlocoPrlACommandValidator()
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

            RuleFor(x => x.Request.Bloco)
                .IsInEnum()
                .WithMessage("O bloco de liquidez deve ser A, B ou C.");
        }
    }
}
