using Application.Commands.Usuarios;
using FluentValidation;

namespace Application.Validators.Usuarios
{
    public class UpdateUsuarioCommandValidator : AbstractValidator<UpdateUsuarioCommand>
    {
        public UpdateUsuarioCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O ID do usuário é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.UpdateUsuarioRequest.Nome)
                .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
                .MaximumLength(255).WithMessage("O nome do usuário não pode exceder 100 caracteres.");

            RuleFor(x => x.UpdateUsuarioRequest.Email)
                .NotEmpty().WithMessage("O e-mail do usuário é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.");

            RuleFor(x => x.UpdateUsuarioRequest.Ativo)
                .NotNull().WithMessage("O status de ativação do usuário deve ser informado.");
        }
    }
}
