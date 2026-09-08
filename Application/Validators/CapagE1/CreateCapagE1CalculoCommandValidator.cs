using Application.Commands.CapagE1;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.CapagE1
{
    public class CreateCapagE1CalculoCommandValidator
        : AbstractValidator<CreateCapagE1CalculoCommand>
    {
        public CreateCapagE1CalculoCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("id_empresa inválido.")
                .When(x => x.Request != null);

            RuleFor(x => x.Request.Modelo)
                .Must(CapagE1CalculoHelper.IsValidModelo)
                .WithMessage("modelo inválido.")
                .When(x => x.Request != null);
        }
    }
}
