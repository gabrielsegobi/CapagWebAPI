using Application.Commands.CapagSimples;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.CapagSimples
{
    public class UpsertManualDemonstrativeValuesCommandValidator
        : AbstractValidator<UpsertManualDemonstrativeValuesCommand>
    {
        public UpsertManualDemonstrativeValuesCommandValidator()
        {
            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("idEmpresa inválido.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Request.DemonstrativeKind)
                .Must(CapagSimplesHelper.IsValidDemonstrativeKind)
                .WithMessage("demonstrativeKind deve ser DRE ou BALANCE_SHEET.")
                .When(x => x.Request != null);
        }
    }
}
