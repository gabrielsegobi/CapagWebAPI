namespace Domain.Enums
{
    public static class StatusComercialEmpresa
    {
        public const string CalculoEfetuado = "calculo_efetuado";
        public const string EmNegociacao = "em_negociacao";
        public const string ContratoFechado = "contrato_fechado";

        public static readonly string[] Todos =
        [
            CalculoEfetuado,
            EmNegociacao,
            ContratoFechado
        ];

        public static bool IsValid(string? value) =>
            value != null && Todos.Contains(value);
    }
}
