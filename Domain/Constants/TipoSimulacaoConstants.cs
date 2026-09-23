namespace Domain.Constants
{
    public static class TipoSimulacaoConstants
    {
        public const string Previdenciario = "PREVIDENCIARIO";
        public const string Demais = "DEMAIS";
        public const string Simples = "SIMPLES";
        public const string Terceiros = "TERCEIROS";
        public const string Fgts = "FGTS";

        public static readonly string[] Todos =
        [
            Previdenciario,
            Demais,
            Simples,
            Terceiros,
            Fgts
        ];

        public const string MySqlColumnType = "VARCHAR(32)";

        public static bool IsValid(string? tipo)
        {
            if (string.IsNullOrWhiteSpace(tipo))
                return false;

            var normalized = tipo.Trim().ToUpperInvariant();
            return Todos.Contains(normalized);
        }

        public static string Normalize(string tipo) => tipo.Trim().ToUpperInvariant();
    }
}
