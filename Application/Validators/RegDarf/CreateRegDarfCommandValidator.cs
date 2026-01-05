using Application.Commands.RegDarf;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.RegDarf
{
    public class CreateRegDarfCommandValidator : AbstractValidator<CreateRegDarfCommand>
    {
        public CreateRegDarfCommandValidator()
        {
            RuleFor(x => x.Requests)
                .NotNull().WithMessage("A lista de DARFs não pode ser nula.")
                .NotEmpty().WithMessage("A lista de DARFs não pode estar vazia.");

            RuleForEach(x => x.Requests)
                .ChildRules(darf =>
                {
                    darf.RuleFor(d => d.IdTenant)
                        .NotEmpty().WithMessage("O ID do tenant é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID do tenant deve ser maior ou igual a zero.");

                    darf.RuleFor(d => d.IdEmpresa)
                        .NotEmpty().WithMessage("O ID da empresa é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O ID da empresa deve ser maior ou igual a zero.");

                    darf.RuleFor(d => d.DataArrecadacao)
                        .NotEmpty().WithMessage("A data de arrecadação é obrigatória.")
                        .Must(data =>
                            data.ToDateTime(TimeOnly.MinValue) <= DateTimeHelper.GetDateTimeNow())
                        .WithMessage("A data de arrecadação não pode ser futura.");

                    darf.RuleFor(d => d.ValorTotal)
                        .NotEmpty().WithMessage("O valor total é obrigatório.")
                        .GreaterThanOrEqualTo(0).WithMessage("O valor total deve ser maior ou igual a zero.");
                });
        }
    }
}
