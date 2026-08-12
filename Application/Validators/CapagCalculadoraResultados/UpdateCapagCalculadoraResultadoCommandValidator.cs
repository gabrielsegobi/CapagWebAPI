using Application.Commands.CapagCalculadoraResultados;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.CapagCalculadoraResultados
{
    public class UpdateCapagCalculadoraResultadoCommandValidator
        : AbstractValidator<UpdateCapagCalculadoraResultadoCommand>
    {
        public UpdateCapagCalculadoraResultadoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID deve ser maior que zero.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.Modelo)
                .NotEmpty()
                .WithMessage("O modelo é obrigatório.")
                .Must(m => CapagCalculadoraModelos.Todos.Contains(m.Trim()))
                .WithMessage($"O modelo deve ser um dos valores: {string.Join(", ", CapagCalculadoraModelos.Todos)}.");

            RuleFor(x => x.Request.Classificacao)
                .NotEmpty()
                .WithMessage("A classificação é obrigatória.")
                .Must(c => CapagCalculadoraClassificacoes.Todos.Contains(c.Trim().ToUpperInvariant()))
                .WithMessage("A classificação deve ser A, B, C ou D.");

            RuleFor(x => x.Request.PercentualExibicao)
                .NotEmpty()
                .WithMessage("O percentual de exibição é obrigatório.")
                .MaximumLength(16)
                .WithMessage("O percentual de exibição deve ter no máximo 16 caracteres.");

            RuleFor(x => x.Request.LabelMetrica)
                .NotEmpty()
                .WithMessage("O rótulo da métrica é obrigatório.")
                .MaximumLength(64)
                .WithMessage("O rótulo da métrica deve ter no máximo 64 caracteres.");

            RuleFor(x => x.Request.StatusMensagem)
                .NotEmpty()
                .WithMessage("A mensagem de status é obrigatória.")
                .MaximumLength(255)
                .WithMessage("A mensagem de status deve ter no máximo 255 caracteres.");
        }
    }
}
