using Application.Commands.Empresas;
using Domain.ValueObjects;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.Empresas
{
    public class CreateEmpresaCommandValidator : AbstractValidator<CreateEmpresaCommand>
    {
        public CreateEmpresaCommandValidator()
        {
            RuleFor(x => x.CreateEmpresaRequest.IdTenant)
                .GreaterThan(0)
                .WithMessage("O ID do tenant é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.CreateEmpresaRequest.Cnpj)
                .NotEmpty()
                .WithMessage("O CNPJ é obrigatório.")
                .Must(cnpj => Cnpj.TentarCriar(cnpj) != null)
                .WithMessage("O CNPJ informado é inválido.");

            RuleFor(x => x.CreateEmpresaRequest.RazaoSocial)
                .NotEmpty()
                .WithMessage("A razão social é obrigatória.")
                .MaximumLength(255)
                .WithMessage("A razão social não pode exceder 255 caracteres.");

            RuleFor(x => x.CreateEmpresaRequest.NomeFantasia)
                .MaximumLength(255)
                .WithMessage("O nome fantasia não pode exceder 255 caracteres.");

            RuleFor(x => x.CreateEmpresaRequest.MatrizFilial)
                .NotEmpty()
                .WithMessage("O tipo (matriz/filial) é obrigatório.")
                .Must(valor => valor.Equals("MATRIZ", StringComparison.OrdinalIgnoreCase)
                            || valor.Equals("FILIAL", StringComparison.OrdinalIgnoreCase))
                .WithMessage("O tipo de empresa deve ser 'MATRIZ' ou 'FILIAL'.");

            RuleFor(x => x.CreateEmpresaRequest.IdEmpresaMatriz)
                .Must((command, idMatriz) =>
                {
                    if (command.CreateEmpresaRequest.MatrizFilial.Equals("filia", StringComparison.OrdinalIgnoreCase))
                        return idMatriz.HasValue;

                    if (command.CreateEmpresaRequest.MatrizFilial.Equals("matriz", StringComparison.OrdinalIgnoreCase))
                        return idMatriz == null;

                    return true;
                })
                .WithMessage("Empresas filiais devem ter uma empresa matriz associada.");

            RuleFor(x => x.CreateEmpresaRequest.Cnae)
                .NotEmpty()
                .WithMessage("O CNAE é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O CNAE não pode exceder 150 caracteres.");

            RuleFor(RuleFor => RuleFor.CreateEmpresaRequest.DataAbertura)
                .LessThanOrEqualTo(DateTimeHelper.GetDateTimeNow())
                .WithMessage("A data de abertura não pode ser uma data futura.");

            RuleFor(x => x.CreateEmpresaRequest.MunicipioEstado)
                .NotEmpty()
                .WithMessage("O campo Município/Estado é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O campo Município/Estado não pode exceder 50 caracteres.");

            RuleFor(x => x.CreateEmpresaRequest.CapitalSocial)
                .NotEmpty()
                .WithMessage("O capital social é obrigatório.")
                .MaximumLength(50)
                .WithMessage("O capital social não pode exceder 50 caracteres.");

            RuleFor(x => x.CreateEmpresaRequest.Segmento)
                .NotEmpty()
                .WithMessage("O segmento é obrigatório.")
                .MaximumLength(100)
                .WithMessage("O segmento não pode exceder 100 caracteres.");

            RuleFor(x => x.CreateEmpresaRequest.Porte)
                .NotEmpty()
                .WithMessage("O porte é obrigatório.")
                .MaximumLength(30)
                .WithMessage("O porte não pode exceder 30 caracteres.");

        }
    }
}
