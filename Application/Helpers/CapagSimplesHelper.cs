using Domain.Enums;

namespace Application.Helpers
{
    public static class CapagSimplesHelper
    {
        public const string KindSimplesExerciseYears = nameof(DeclarationKind.SIMPLES_EXERCISE_YEARS);
        public const string KindNoNationalSimpleStrict = nameof(DeclarationKind.NO_NATIONAL_SIMPLE_STRICT);
        public const string KindDre = nameof(DemonstrativeKind.DRE);
        public const string KindBalanceSheet = nameof(DemonstrativeKind.BALANCE_SHEET);

        public static bool IsValidDeclarationKind(string? kind) =>
            string.Equals(kind, KindSimplesExerciseYears, StringComparison.Ordinal) ||
            string.Equals(kind, KindNoNationalSimpleStrict, StringComparison.Ordinal);

        public static bool IsValidDemonstrativeKind(string? kind) =>
            string.Equals(kind, KindDre, StringComparison.Ordinal) ||
            string.Equals(kind, KindBalanceSheet, StringComparison.Ordinal);

        public static bool IsValidExerciseYear(int year) => year > 1900 && year < 2100;

        /// <summary>
        /// Filtra anos válidos, remove duplicados e ordena.
        /// </summary>
        public static List<int> NormalizeExerciseYears(IEnumerable<int>? years)
        {
            if (years == null)
                return [];

            return years
                .Where(IsValidExerciseYear)
                .Distinct()
                .OrderBy(y => y)
                .ToList();
        }

        public static Dictionary<string, Dictionary<string, decimal>> ToPorCodigoMap(
            IEnumerable<(string AccountCode, int Year, decimal Value)> rows)
        {
            var result = new Dictionary<string, Dictionary<string, decimal>>(StringComparer.Ordinal);

            foreach (var row in rows)
            {
                if (!result.TryGetValue(row.AccountCode, out var byYear))
                {
                    byYear = new Dictionary<string, decimal>(StringComparer.Ordinal);
                    result[row.AccountCode] = byYear;
                }

                byYear[row.Year.ToString()] = row.Value;
            }

            return result;
        }

        /// <summary>
        /// Expande porCodigo em linhas normalizadas (trim, anos válidos, códigos não vazios).
        /// </summary>
        public static List<(string AccountCode, int Year, decimal Value)> FlattenPorCodigo(
            Dictionary<string, Dictionary<string, decimal>>? porCodigo)
        {
            var rows = new List<(string, int, decimal)>();
            if (porCodigo == null || porCodigo.Count == 0)
                return rows;

            foreach (var (rawCode, years) in porCodigo)
            {
                var code = (rawCode ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(code) || years == null)
                    continue;

                foreach (var (yearKey, value) in years)
                {
                    if (!int.TryParse(yearKey, out var year) || !IsValidExerciseYear(year))
                        continue;

                    rows.Add((code, year, value));
                }
            }

            return rows;
        }
    }
}
