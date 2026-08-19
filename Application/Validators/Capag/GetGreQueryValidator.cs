using Application.Queries.Capag;
using FluentValidation;

namespace Application.Validators.Capag
{
    public class GetGreQueryValidator : AbstractValidator<GetGreQuery>
    {
        public GetGreQueryValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");
        }
    }
}
