using Application.Queries.CapagFco;
using FluentValidation;

namespace Application.Validators.CapagFco
{
    public class GetCapagFcoParametrosQueryValidator : AbstractValidator<GetCapagFcoParametrosQuery>
    {
        public GetCapagFcoParametrosQueryValidator()
        {
            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("idEmpresa inválido.");
        }
    }
}
