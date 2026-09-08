using Application.Commands.CapagE1;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.CapagE1
{
    public class UpdateCapagE1CalculoCommandValidator
        : AbstractValidator<UpdateCapagE1CalculoCommand>
    {
        public UpdateCapagE1CalculoCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("id inválido.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("id_empresa inválido.")
                .When(x => x.Request != null && x.Request.IdEmpresa != 0);

            RuleFor(x => x.Request.Modelo)
                .Must(CapagE1CalculoHelper.IsValidModelo)
                .WithMessage("modelo inválido.")
                .When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.Modelo));
        }
    }
}
