using Application.Commands.DescricaoDebitos;
using FluentValidation;

namespace Application.Validators.DescricaoDebitos
{
    public class UpdateDescricaoDebitoCommandValidator
        : AbstractValidator<UpdateDescricaoDebitoCommand>
    {
        public UpdateDescricaoDebitoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID do débito deve ser maior que zero.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.Natureza)
                .NotEmpty()
                .WithMessage("A natureza do débito é obrigatória.")
                .MaximumLength(100)
                .WithMessage("A natureza do débito deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Request.NumCda)
                .NotEmpty()
                .WithMessage("O número da CDA é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O número da CDA deve ter no máximo 50 caracteres.");

            RuleFor(x => x.Request.DataInscricao)
                .NotEmpty()
                .WithMessage("A data de inscrição é obrigatória.")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("A data de inscrição não pode ser maior que a data atual.");

            RuleFor(x => x.Request.ValorPrincipal)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor principal deve ser maior ou igual a zero.");

            RuleFor(x => x.Request.ValorMulta)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor da multa deve ser maior ou igual a zero.");

            RuleFor(x => x.Request.ValorJuros)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor dos juros deve ser maior ou igual a zero.");

            RuleFor(x => x.Request.ValorEncargos)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor dos encargos deve ser maior ou igual a zero.");

            RuleFor(x => x.Request)
                .Must(r => r.ValorPrincipal + r.ValorMulta + r.ValorJuros + r.ValorEncargos > 0)
                .WithMessage("O débito deve possuir ao menos um valor maior que zero.");
        }
    }
}
