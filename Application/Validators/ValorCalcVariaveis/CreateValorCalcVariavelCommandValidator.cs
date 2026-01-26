using Application.Commands.ValorCalcVariaveis;
using FluentValidation;

namespace Application.Validators.ValorCalcVariaveis
{
    public class CreateValorCalcVariavelCommandValidator
        : AbstractValidator<CreateValorCalcVariavelCommand>
    {
        public CreateValorCalcVariavelCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Request.IdTipoGrupo)
                .GreaterThan(0)
                .WithMessage("O ID do tipo de grupo deve ser maior que zero.");

            RuleFor(x => x.Request.AnoBase)
                .GreaterThan((short)0)
                .WithMessage("O ano base deve ser válido.")
                .InclusiveBetween((short)1900, (short)2100)
                .WithMessage("O ano base deve estar entre 1900 e 2100.");

            RuleFor(x => x.Request.IdVariavel)
                .NotEmpty()
                .WithMessage("O identificador da variável é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O identificador da variável deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Request.Valor)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor deve ser maior ou igual a zero.");
            RuleFor(x => x.Request.Status)
                .NotEmpty()
                .WithMessage("O status é obrigatório.")
                .Must(s => s == "ignorado" || s == "preenchido")
                .WithMessage("O status deve ser 'ignorado' ou 'preenchido'.");
        }
    }
}
