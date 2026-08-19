namespace Application.Helpers
{
    /// <summary>
    /// PL (<c>2.03</c>) negativo ou zero: o ROE não é interpretável.
    /// Os demais indicadores continuam usando o valor assinado do PL na fórmula.
    /// </summary>
    public static class IndicadorAlertaPlHelper
    {
        public const string Mensagem = "Não Analisar: Informação Comprometida";
        public const string NomeRoe = "Retorno sobre o Patrimônio Líquido (ROE)";
        public const string CodigoPl = "2.03";

        public static bool EhRoe(string? nomeIndicador) =>
            string.Equals(nomeIndicador, NomeRoe, StringComparison.OrdinalIgnoreCase);

        public static bool PlNegativoOuZero(IReadOnlyDictionary<string, double> valoresAno) =>
            valoresAno.TryGetValue(CodigoPl, out var pl) && pl <= 0;

        /// <summary>
        /// Só o ROE interrompe o cálculo. CGLP, grau de endividamento etc. seguem com o PL negativo.
        /// </summary>
        public static bool DeveOmitirCalculo(string? nomeIndicador, IReadOnlyDictionary<string, double> valoresAno) =>
            EhRoe(nomeIndicador) && PlNegativoOuZero(valoresAno);
    }
}
