using Application.Commands.Usuarios;
using FluentValidation;

namespace Application.Validators.Usuarios
{
    public class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
    {
        public CreateUsuarioCommandValidator()
        {
            RuleFor(x => x.CreateUsuarioRequest.Nome)
                .NotEmpty().WithMessage("O nome do usuário é obrigatório.")
                .MaximumLength(255).WithMessage("O nome do usuário não pode exceder 100 caracteres.");

            RuleFor(x => x.CreateUsuarioRequest.Email)
                .NotEmpty().WithMessage("O e-mail do usuário é obrigatório.")
                .EmailAddress().WithMessage("O e-mail informado não é válido.");

            RuleFor(x => x.CreateUsuarioRequest.SenhaHash)
                .NotEmpty().WithMessage("A senha do usuário é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");
        }
    }
}
