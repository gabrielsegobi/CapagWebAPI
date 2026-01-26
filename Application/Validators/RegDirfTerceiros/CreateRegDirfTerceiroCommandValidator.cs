using Application.Commands.RegDirfTerceiros;
using FluentValidation;

namespace Application.Validators.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroCommandValidator
        : AbstractValidator<CreateRegDirfTerceiroCommand>
    {
        public CreateRegDirfTerceiroCommandValidator()
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
                .WithMessage("A lista de registros DIRF não pode ser nula.")
                .NotEmpty()
                .WithMessage("A lista de registros DIRF não pode estar vazia.");

            RuleForEach(x => x.Requests.Requests)
                .ChildRules(req =>
                {
                    req.RuleFor(d => d.IdEmpresa)
                        .GreaterThan(0)
                        .WithMessage("O ID da empresa deve ser maior que zero.");

                    req.RuleFor(d => d.Codigo)
                        .NotEmpty()
                        .WithMessage("O código é obrigatório.")
                        .MaximumLength(10)
                        .WithMessage("O código deve ter no máximo 10 caracteres.");

                    req.RuleFor(d => d.ValorRendimento)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O valor do rendimento deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.ValorTributo)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O valor do tributo deve ser maior ou igual a zero.");

                    req.RuleFor(d => d.AnoCalendario)
                        .NotEmpty()
                        .WithMessage("O ano calendário é obrigatório.")
                        .Length(4)
                        .WithMessage("O ano calendário deve conter 4 dígitos.");

                    req.RuleFor(d => d.DataProcessamento)
                        .NotEmpty()
                        .WithMessage("A data de processamento é obrigatória.");
                });
        }
    }
}
