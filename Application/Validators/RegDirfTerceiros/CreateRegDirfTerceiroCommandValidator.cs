using Application.Commands.RegDirfTerceiros;
using FluentValidation;

namespace Application.Validators.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroCommandValidator : AbstractValidator<CreateRegDirfTerceiroCommand>
    {
        public CreateRegDirfTerceiroCommandValidator()
        {
            RuleFor(x => x.Requests)
                .NotNull().WithMessage("A lista de registros DIRF não pode ser nula.")
                .NotEmpty().WithMessage("A lista de registros DIRF não pode estar vazia.");

            RuleForEach(x => x.Requests)
                .ChildRules(req =>
                {
                    req.RuleFor(d => d.IdTenant)
                        .NotEmpty().WithMessage("O ID do tenant é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID do tenant deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.IdEmpresa)
                        .NotEmpty().WithMessage("O ID da empresa é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID da empresa deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.Codigo)
                        .NotEmpty().WithMessage("O código é obrigatório.")
                        .MaximumLength(10).WithMessage("O código deve ter no máximo 10 caracteres.");

                    req.RuleFor(d => d.ValorRendimento)
                        .NotEmpty().WithMessage("O valor do rendimento é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O valor do rendimento deve ser maior ou igual a zero.");
                });
        }
    }
}
