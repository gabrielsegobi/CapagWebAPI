using Application.Commands.CapagFco;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.CapagFco
{
    public class UpsertCapagFcoParametrosCommandValidator
        : AbstractValidator<UpsertCapagFcoParametrosCommand>
    {
        public UpsertCapagFcoParametrosCommandValidator()
        {
            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("idEmpresa inválido.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Request.Bloco)
                .Must(CapagFcoParametrosHelper.IsValidBloco)
                .WithMessage("bloco deve ser l100 ou l300.")
                .When(x => x.Request != null);

            RuleFor(x => x.Request.Excecoes)
                .NotNull()
                .WithMessage("excecoes é obrigatório (objeto vazio limpa o bloco).")
                .When(x => x.Request != null);

            RuleFor(x => x.Request.VersaoBase)
                .MaximumLength(CapagFcoParametrosHelper.MaxVersaoBaseLength)
                .WithMessage($"versaoBase deve ter no máximo {CapagFcoParametrosHelper.MaxVersaoBaseLength} caracteres.")
                .When(x => x.Request != null && !string.IsNullOrWhiteSpace(x.Request.VersaoBase));
        }
    }
}
