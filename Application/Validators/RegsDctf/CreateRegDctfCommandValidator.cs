using Application.Commands.RegsDctf;
using FluentValidation;

namespace Application.Validators.RegsDctf
{
    public class CreateRegDctfCommandValidator
        : AbstractValidator<CreateRegDctfCommand>
    {
        public CreateRegDctfCommandValidator()
        {
            RuleFor(x => x.Requests)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Requests.FileName)
                .NotEmpty()
                .WithMessage("O nome do arquivo é obrigatório.");

            RuleFor(x => x.Requests.Type)
                .NotEmpty()
                .WithMessage("O tipo é obrigatório.");

            RuleFor(x => x.Requests.Requests)
                .NotNull()
                .WithMessage("A lista de registros DCTF não pode ser nula.")
                .NotEmpty()
                .WithMessage("A lista de registros DCTF não pode estar vazia.");

            RuleForEach(x => x.Requests.Requests)
                .ChildRules(req =>
                {
                    req.RuleFor(d => d.IdEmpresa)
                        .GreaterThan(0)
                        .WithMessage("O ID da empresa deve ser maior que zero.");

                    req.RuleFor(d => d.Valor)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("O valor deve ser maior ou igual a zero.");
                });
        }
    }
}
