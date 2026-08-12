using Application.Commands.CapagSimples;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators.CapagSimples
{
    public class UpsertSimplesDeclarationCommandValidator : AbstractValidator<UpsertSimplesDeclarationCommand>
    {
        public UpsertSimplesDeclarationCommandValidator()
        {
            RuleFor(x => x.IdEmpresa)
                .GreaterThan(0)
                .WithMessage("idEmpresa inválido.");

            RuleFor(x => x.Request)
                .NotNull()
                .WithMessage("O corpo da requisição é obrigatório.");

            RuleFor(x => x.Request.DeclarationKind)
                .Must(CapagSimplesHelper.IsValidDeclarationKind)
                .WithMessage("declarationKind deve ser SIMPLES_EXERCISE_YEARS ou NO_NATIONAL_SIMPLE_STRICT.")
                .When(x => x.Request != null);

            RuleFor(x => x.Request)
                .Must(r =>
                {
                    if (r.DeclarationKind != CapagSimplesHelper.KindSimplesExerciseYears)
                        return true;

                    var years = CapagSimplesHelper.NormalizeExerciseYears(r.ExerciseYears);
                    return years.Count > 0;
                })
                .WithMessage("SIMPLES_EXERCISE_YEARS exige pelo menos 1 exercício válido (1900 < ano < 2100).")
                .When(x => x.Request != null && CapagSimplesHelper.IsValidDeclarationKind(x.Request.DeclarationKind));
        }
    }
}
