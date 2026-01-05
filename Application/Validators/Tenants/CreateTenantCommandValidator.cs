using Application.Commands.Tenants;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Tenants
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.CreateTenantRequest.Nome)
                .NotNull().WithMessage("O nome é obrigatório")
                .MaximumLength(255).WithMessage("O nome não pode exceder 255 caracteres.");

            RuleFor(x => x.CreateTenantRequest.Nome)
                .NotNull().WithMessage("O Slug é obrigatório")
                .MaximumLength(100).WithMessage("O slug não pode exceder 100 caracteres.");

            RuleFor(x => x.CreateTenantRequest.Plano)
                .NotEmpty().WithMessage("O plano é obrigatório.")
                .IsEnumName(typeof(TenantPlanosEnum), caseSensitive: false)
                .WithMessage("O plano informado é inválido. Os valores permitidos são: basico, profissional, enterprise.");

            RuleFor(x => x.CreateTenantRequest.Status)
                .NotEmpty().WithMessage("O status é obrigatório.")
                .IsEnumName(typeof(TenantStatusEnum), caseSensitive: false)
                .WithMessage("O status informado é inválido. Os valores permitidos são: ativo, suspenso, cancelado, trial.");
        }
    }
}
