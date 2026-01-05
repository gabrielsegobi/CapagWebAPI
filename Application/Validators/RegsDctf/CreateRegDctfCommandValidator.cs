using Application.Commands.RegsDctf;
using FluentValidation;

namespace Application.Validators.RegsDctf
{
    public class CreateRegDctfCommandValidator : AbstractValidator<CreateRegDctfCommand>
    {
        public CreateRegDctfCommandValidator()
        {
            RuleFor(x => x.Requests)
                .NotNull().WithMessage("A lista de registros DCTF não pode ser nula.")
                .NotEmpty().WithMessage("A lista de registros DCTF não pode estar vazia.");

            RuleForEach(x => x.Requests)
                .ChildRules(req =>
                {
                    req.RuleFor(d => d.IdTenant)
                        .NotEmpty().WithMessage("O ID do tenant é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID do tenant deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.IdEmpresa)
                        .NotEmpty().WithMessage("O ID da empresa é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID da empresa deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.Valor)
                        .NotEmpty().WithMessage("O valor é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O valor deve ser maior ou igual a zero.");
                });
        }
    }
}
