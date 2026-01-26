using Application.Commands.ICPAnterior;
using FluentValidation;

namespace Application.Validators.ICPAnterior
{
    public class CreateICPAnteriorCommandValidator
        : AbstractValidator<CreateICPAnteriorCommand>
    {
        public CreateICPAnteriorCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Request.Classificacao)
                .NotNull()
                .WithMessage("A classificação é obrigatória.");
                //.Must(c => c == 'A' || c == 'B' || c == 'C')
                //.WithMessage("A classificação deve ser A, B ou C.");

            RuleFor(x => x.Request.ValorICPReceita)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O valor do ICP receita deve ser maior ou igual a zero.");
        }
    }
}
