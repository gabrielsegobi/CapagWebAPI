namespace Application.Helpers
{
    public static class CnpjRootHelper
    {
        public static string DigitsOnly(string? cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return string.Empty;

            return new string(cnpj.Where(char.IsDigit).ToArray());
        }

        /// <summary>Retorna a raiz do CNPJ (8 primeiros dígitos), ou null se insuficiente.</summary>
        public static string? ToRoot(string? cnpj)
        {
            var digits = DigitsOnly(cnpj);
            return digits.Length >= 8 ? digits[..8] : null;
        }

        public static HashSet<string> ParseRoots(IEnumerable<string>? values)
        {
            var roots = new HashSet<string>(StringComparer.Ordinal);
            if (values == null)
                return roots;

            foreach (var value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                foreach (var part in value.Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    var root = ToRoot(part);
                    if (root != null)
                        roots.Add(root);
                }
            }

            return roots;
        }

        public static bool MatchesRoot(string? cnpj, IReadOnlySet<string> allowedRoots)
        {
            if (allowedRoots == null || allowedRoots.Count == 0)
                return false;

            var root = ToRoot(cnpj);
            return root != null && allowedRoots.Contains(root);
        }
    }
}
