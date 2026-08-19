using Application.Queries.Carteira;
using FluentValidation;

namespace Application.Validators.Carteira
{
    public class GetCarteiraRatingQueryValidator : AbstractValidator<GetCarteiraRatingQuery>
    {
        private static readonly string FormatoEsperado = "MM/yyyy";

        public GetCarteiraRatingQueryValidator()
        {
            RuleFor(x => x.MesDe)
                .Must(BeValidMonthYear)
                .When(x => !string.IsNullOrWhiteSpace(x.MesDe))
                .WithMessage($"mesDe deve estar no formato {FormatoEsperado} (ex: 02/2026).");

            RuleFor(x => x.MesAte)
                .Must(BeValidMonthYear)
                .When(x => !string.IsNullOrWhiteSpace(x.MesAte))
                .WithMessage($"mesAte deve estar no formato {FormatoEsperado} (ex: 07/2026).");
        }

        private static bool BeValidMonthYear(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            return DateOnly.TryParseExact(
                $"01/{value}",
                "dd/MM/yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _);
        }
    }
}
