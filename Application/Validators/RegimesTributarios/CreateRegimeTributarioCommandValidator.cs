using Application.Commands.RegimesTributarios;
using FluentValidation;

namespace Application.Validators.RegimesTributarios
{
    public class CreateRegimeTributarioCommandValidator : AbstractValidator<CreateRegimeTributarioCommand>
    {
        public CreateRegimeTributarioCommandValidator()
        {
            RuleFor(x => x.CreateRegimeTributarioRequest.IdTenant)
                .GreaterThan(0).WithMessage("O identificador do tenant é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.CreateRegimeTributarioRequest.IdEmpresa)
                .GreaterThan(0).WithMessage("O identificador da empresa é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.CreateRegimeTributarioRequest.Ano)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .WithMessage("O ano informado é inválido.");

            RuleFor(x => x.CreateRegimeTributarioRequest.RaizCnpj)
                .NotEmpty().WithMessage("A raiz do CNPJ é obrigatória.")
                .Length(8, 8).WithMessage("A raiz do CNPJ deve conter exatamente 8 dígitos.")
                .Matches("^[0-9]+$").WithMessage("A raiz do CNPJ deve conter apenas números.");

            RuleFor(x => x.CreateRegimeTributarioRequest.DtIni)
                .NotNull().WithMessage("A data inicial é obrigatória.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now))
                .WithMessage("A data inicial não pode ser futura.");

            RuleFor(x => x.CreateRegimeTributarioRequest.FormaTribCompleta)
                .MaximumLength(100).WithMessage("A forma de tributação não pode exceder 100 caracteres.");

            RuleFor(x => x.CreateRegimeTributarioRequest.FormaApurCompleta)
                .MaximumLength(100).WithMessage("A forma de apuração não pode exceder 100 caracteres.");
        }
    }
}
