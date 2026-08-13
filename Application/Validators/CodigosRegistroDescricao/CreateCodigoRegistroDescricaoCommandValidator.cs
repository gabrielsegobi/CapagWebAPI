using Application.Commands.CodigosRegistroDescricao;
using FluentValidation;

namespace Application.Validators.CodigosRegistroDescricao
{
    public class CreateCodigoRegistroDescricaoCommandValidator
        : AbstractValidator<CreateCodigoRegistroDescricaoCommand>
    {
        public CreateCodigoRegistroDescricaoCommandValidator()
        {
            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O objeto de requisição não pode ser nulo.");

            RuleFor(x => x.Request.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Request.Codigo)
                .NotEmpty()
                .WithMessage("O código é obrigatório.")
                .MaximumLength(20)
                .WithMessage("O código deve ter no máximo 20 caracteres.");

            RuleFor(x => x.Request.ExpressaoRegular)
                .MaximumLength(150)
                .WithMessage("A expressão regular deve ter no máximo 150 caracteres.");
        }
    }
}
