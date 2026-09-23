using Application.Commands.SimulacoesCalc;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.SimulacoesCalc
{
    public class CreateSimulacaoCalcCommandValidator : AbstractValidator<CreateSimulacaoCalcCommand>
    {
        public CreateSimulacaoCalcCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Request.TipoSimulacao)
                .NotEmpty()
                .WithMessage("O tipo de simulação é obrigatório.")
                .Must(TipoSimulacaoConstants.IsValid)
                .WithMessage($"O tipo de simulação deve ser um dos valores: {string.Join(", ", TipoSimulacaoConstants.Todos)}.");
        }
    }
}
