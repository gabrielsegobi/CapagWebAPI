using Application.Commands.Empresas;
using Domain.ValueObjects;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.Empresas
{
    public class UpdateEmpresaCommandValidator : AbstractValidator<UpdateEmpresaCommand>
    {
        public UpdateEmpresaCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.UpdateEmpresaRequest.IdTenant)
                .GreaterThan(0)
                .WithMessage("O ID do tenant é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.UpdateEmpresaRequest.Cnpj)
               .NotEmpty()
               .WithMessage("O CNPJ é obrigatório.")
               .Must(cnpj => Cnpj.TentarCriar(cnpj) != null)
               .WithMessage("O CNPJ informado é inválido.");

            RuleFor(x => x.UpdateEmpresaRequest.RazaoSocial)
                .NotEmpty().WithMessage("A razão social é obrigatória.")
                .MaximumLength(255).WithMessage("A razão social não pode exceder 255 caracteres.");

            RuleFor(x => x.UpdateEmpresaRequest.NomeFantasia)
                .MaximumLength(255).WithMessage("O nome fantasia não pode exceder 255 caracteres.");

            RuleFor(x => x.UpdateEmpresaRequest.MatrizFilial)
                .NotEmpty().WithMessage("O tipo da empresa é obrigatório.")
                .Must(valor => valor.Equals("matriz", StringComparison.OrdinalIgnoreCase)
                            || valor.Equals("filial", StringComparison.OrdinalIgnoreCase))
                .WithMessage("O tipo de empresa deve ser 'MATRIZ' ou 'FILIAL'.");

            RuleFor(x => x.UpdateEmpresaRequest.IdEmpresaMatriz)
                .NotNull()
                .When(x => x.UpdateEmpresaRequest.MatrizFilial.Equals("filial", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Empresas filiais devem ter uma empresa matriz associada.");

            RuleFor(x => x.UpdateEmpresaRequest.Cnae)
                .NotEmpty()
                .WithMessage("O CNAE é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O CNAE não pode exceder 150 caracteres.");

            RuleFor(RuleFor => RuleFor.UpdateEmpresaRequest.DataAbertura)
                .LessThanOrEqualTo(DateTimeHelper.GetDateTimeNow())
                .WithMessage("A data de abertura não pode ser uma data futura.");

            RuleFor(x => x.UpdateEmpresaRequest.MunicipioEstado)
                .NotEmpty()
                .WithMessage("O campo Município/Estado é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo Município/Estado não pode exceder 50 caracteres.");

            RuleFor(x => x.UpdateEmpresaRequest.CapitalSocial)
                .NotEmpty()
                .WithMessage("O capital social é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O capital social não pode exceder 50 caracteres.");

            RuleFor(x => x.UpdateEmpresaRequest.Segmento)
                .NotEmpty()
                .WithMessage("O segmento é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O segmento não pode exceder 100 caracteres.");

            RuleFor(x => x.UpdateEmpresaRequest.Porte)
                .NotEmpty()
                .WithMessage("O porte é obrigatório.")
                .MaximumLength(30)
                .WithMessage("O porte não pode exceder 30 caracteres.");
        }
    }
}
