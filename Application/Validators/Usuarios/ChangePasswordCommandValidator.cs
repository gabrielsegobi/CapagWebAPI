using Application.Commands.Usuarios;
using FluentValidation;

namespace Application.Validators.Usuarios
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("O ID do usuário é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.ChangePasswordRequest)
                .NotNull().WithMessage("Os dados para alteração de senha são obrigatórios.");


            RuleFor(x => x.ChangePasswordRequest.SenhaAtual)
                .NotEmpty().WithMessage("A senha atual é obrigatória.");

            RuleFor(x => x.ChangePasswordRequest.NovaSenha)
                .NotEmpty().WithMessage("A nova senha é obrigatória.")
                .MinimumLength(6).WithMessage("A nova senha deve ter no mínimo 6 caracteres.");
        }
    }
}
