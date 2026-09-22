using Application.Commands.Capag;
using FluentValidation;

namespace Application.Validators.Capag
{
    public class DeleteContaGreManualCommandValidator : AbstractValidator<DeleteContaGreManualCommand>
    {
        public DeleteContaGreManualCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa deve ser maior que zero.");

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("O ID da conta manual deve ser maior que zero.");
        }
    }
}
