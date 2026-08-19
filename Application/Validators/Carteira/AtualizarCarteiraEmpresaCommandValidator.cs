using Application.Commands.Carteira;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Carteira
{
    public class AtualizarCarteiraEmpresaCommandValidator : AbstractValidator<AtualizarCarteiraEmpresaCommand>
    {
        public AtualizarCarteiraEmpresaCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.Request.Status)
                .NotEmpty()
                .WithMessage("O status é obrigatório.")
                .Must(StatusComercialEmpresa.IsValid)
                .WithMessage($"Status inválido. Valores permitidos: {string.Join(", ", StatusComercialEmpresa.Todos)}.");

            RuleFor(x => x.Request.ValorContrato)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Request.ValorContrato.HasValue)
                .WithMessage("O valor do contrato deve ser maior ou igual a zero.");
        }
    }
}
