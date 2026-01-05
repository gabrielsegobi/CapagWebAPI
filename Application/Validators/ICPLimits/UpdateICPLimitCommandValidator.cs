using Application.Commands.ICPLimits;
using FluentValidation;

namespace Application.Validators.ICPLimits
{
    public class UpdateICPLimitCommandValidator : AbstractValidator<UpdateICPLimitCommand>
    {
        public UpdateICPLimitCommandValidator()
        {
            RuleFor(x => x.Id)
               .GreaterThanOrEqualTo(0).WithMessage("O ID é obrigatório e deve ser zero ou maior.");

            RuleFor(x => x.UpdateICPLimitRequest.Label)
                .MaximumLength(120).WithMessage("O campo Label deve ter no máximo 120 caracteres.");

            RuleFor(x => x.UpdateICPLimitRequest.MinValue)
                .NotNull().WithMessage("O valor mínimo é obrigatório.");

            RuleFor(x => x.UpdateICPLimitRequest.MaxValue)
                .NotNull().WithMessage("O valor máximo é obrigatório.");
          
        }
    }
}
