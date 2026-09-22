using Application.Commands.Capag;
using FluentValidation;

namespace Application.Validators.Capag
{
    public class PatchContaInversaoCommandValidator : AbstractValidator<PatchContaInversaoCommand>
    {
        public PatchContaInversaoCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.CodigoConta)
                .NotEmpty()
                .WithMessage("O código da conta é obrigatório.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.Ano)
                .InclusiveBetween(1990, 2100)
                .WithMessage("O ano do exercício é inválido.");

            RuleFor(x => x.Request.Justificativa)
                .NotEmpty()
                .When(x => x.Request is { Invertido: true })
                .WithMessage("A justificativa é obrigatória ao inverter o sinal.");

            RuleFor(x => x.Request.Justificativa)
                .MaximumLength(500)
                .When(x => x.Request?.Justificativa != null)
                .WithMessage("A justificativa deve ter no máximo 500 caracteres.");
        }
    }
}
