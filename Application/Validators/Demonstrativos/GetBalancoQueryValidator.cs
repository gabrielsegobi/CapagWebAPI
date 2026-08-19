using Application.Queries.Demonstrativos;
using FluentValidation;

namespace Application.Validators.Demonstrativos
{
    public class GetBalancoQueryValidator : AbstractValidator<GetBalancoQuery>
    {
        public GetBalancoQueryValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");
        }
    }
}
