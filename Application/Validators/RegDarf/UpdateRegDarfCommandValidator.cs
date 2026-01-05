using Application.Commands.RegDarf;
using FluentValidation;
using Infrastructure.Helpers;

namespace Application.Validators.RegDarf
{
    public class UpdateRegDarfCommandValidator : AbstractValidator<UpdateRegDarfCommand>
    {
        public UpdateRegDarfCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O ID do registro é obrigatório.")
                .GreaterThan(0).WithMessage("O ID do registro deve ser maior que zero.");

            RuleFor(x => x.Request.DataArrecadacao)
                .NotEmpty().WithMessage("A data de arrecadação é obrigatória.")
                .Must(data =>
                    data.ToDateTime(TimeOnly.MinValue) <= DateTimeHelper.GetDateTimeNow())
                .WithMessage("A data de arrecadação não pode ser uma data futura.");

            RuleFor(x => x.Request.ValorTotal)
                .NotEmpty().WithMessage("O valor total é obrigatório.")
                .GreaterThanOrEqualTo(0).WithMessage("O valor total deve ser maior ou igual a zero.");
        }
    }
}
