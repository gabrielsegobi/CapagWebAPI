using Application.Helpers;
using Application.Queries.CapagE1;
using FluentValidation;

namespace Application.Validators.CapagE1
{
    public class GetCapagE1CalculoQueryValidator : AbstractValidator<GetCapagE1CalculoQuery>
    {
        public GetCapagE1CalculoQueryValidator()
        {
            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("IdEmpresa inválido.");

            RuleFor(x => x.Modelo)
                .Must(CapagE1CalculoHelper.IsValidModelo)
                .WithMessage("modelo inválido.")
                .When(x => !string.IsNullOrWhiteSpace(x.Modelo));
        }
    }
}
