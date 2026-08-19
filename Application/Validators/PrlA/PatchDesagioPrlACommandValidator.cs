using Application.Commands.PrlA;
using FluentValidation;

namespace Application.Validators.PrlA
{
    public class PatchDesagioPrlACommandValidator : AbstractValidator<PatchDesagioPrlACommand>
    {
        public PatchDesagioPrlACommandValidator()
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

            RuleFor(x => x.Request.PercentualDesagio)
                .InclusiveBetween(0m, 100m)
                .WithMessage("O percentual de deságio deve estar entre 0 e 100.");
        }
    }
}
