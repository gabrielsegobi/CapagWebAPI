using Application.Commands.PrlA;
using Domain.Constants;
using FluentValidation;

namespace Application.Validators.PrlA
{
    public class PatchAcaoPrlACommandValidator : AbstractValidator<PatchAcaoPrlACommand>
    {
        public PatchAcaoPrlACommandValidator()
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

            RuleFor(x => x.Request.Acao)
                .Must(PrlAConstants.IsAcaoValida)
                .WithMessage("A ação deve ser incluir, incluir_com_desagio ou excluir.");

            RuleFor(x => x.Request.Justificativa)
                .NotEmpty()
                .When(x => x.Request?.Acao == PrlAConstants.AcaoExcluir)
                .WithMessage("A justificativa é obrigatória ao excluir a conta.");

            RuleFor(x => x.Request.Justificativa)
                .MaximumLength(500)
                .When(x => x.Request?.Justificativa != null)
                .WithMessage("A justificativa deve ter no máximo 500 caracteres.");
        }
    }
}
