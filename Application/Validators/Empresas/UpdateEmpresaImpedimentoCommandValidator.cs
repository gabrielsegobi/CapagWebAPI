using Application.Commands.Empresas;
using FluentValidation;

namespace Application.Validators.Empresas
{
    public class UpdateEmpresaImpedimentoCommandValidator : AbstractValidator<UpdateEmpresaImpedimentoCommand>
    {
        public UpdateEmpresaImpedimentoCommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("O ID da empresa é obrigatório e deve ser maior que zero.");

            RuleFor(x => x.UpdateEmpresaImpedimentoRequest)
                .NotNull()
                .WithMessage("Os dados de impedimento são obrigatórios.");

            RuleFor(x => x.UpdateEmpresaImpedimentoRequest.DataImpedimento)
                .NotEmpty()
                .WithMessage("A data de impedimento é obrigatória.");

            RuleFor(x => x.UpdateEmpresaImpedimentoRequest.IdUsuarioResponsavel)
                .GreaterThan(0)
                .WithMessage("O ID do usuário responsável é obrigatório e deve ser maior que zero.");
        }
    }
}
