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
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Requests.FileName)
                .NotEmpty()
                .WithMessage("O nome do arquivo é obrigatório.");

            RuleFor(x => x.Requests.Type)
                .NotEmpty()
                .WithMessage("O tipo é obrigatório.");

            RuleFor(x => x.Requests.Requests)
                .NotNull()
                .WithMessage("A lista de DARFs não pode ser nula.")
                .NotEmpty()
                .WithMessage("A lista de DARFs não pode estar vazia.");

            RuleForEach(x => x.Requests.Requests)
                .ChildRules(darf =>
                {
                    darf.RuleFor(d => d.IdEmpresa)
                        .GreaterThan(0)
                        .WithMessage("O ID da empresa deve ser maior que zero.");

                    darf.RuleFor(d => d.DataArrecadacao)
                        .NotEmpty()
                        .WithMessage("A data de arrecadação é obrigatória.")
                        .Must(data =>
                            data.ToDateTime(TimeOnly.MinValue) <= DateTimeHelper.GetDateTimeNow())
                        .WithMessage("A data de arrecadação não pode ser futura.");

                    darf.RuleFor(d => d.ValorTotal)
                        .GreaterThan(0)
                        .WithMessage("O valor total deve ser maior que zero.");
                });
        }
    }
}
