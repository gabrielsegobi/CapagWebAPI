namespace Application.Helpers
{
    /// <summary>
    /// Escolhe o registro ECD de fechamento do exercício.
    /// <c>{codigo}[I]</c> é o fechamento do ano anterior: T04 se trimestral, A00 se anual.
    /// Sem esse registro, o saldo inicial é 0.
    /// </summary>
    public static class DemonstrativoPeriodoHelper
    {
        public static bool EhTrimestral(IEnumerable<string?> periodos) =>
            periodos.Any(p => p != null && p.StartsWith("T0", StringComparison.Ordinal));

        public static T? FechamentoDoExercicio<T>(IReadOnlyList<T> items, Func<T, string?> perApur)
        {
            if (items.Count == 0)
                return default;

            var a00 = items.FirstOrDefault(x => perApur(x) == "A00");
            var t04 = items.FirstOrDefault(x => perApur(x) == "T04");
            var trimestral = EhTrimestral(items.Select(perApur));
            return trimestral ? t04 ?? a00 : a00 ?? items[0];
        }

        /// <summary>
        /// Registro cujo <c>ValCtaRefFin</c> alimenta o <c>[I]</c> do ano seguinte.
        /// Trimestral exige T04; anual exige A00. Sem o período, retorna nulo (saldo inicial 0).
        /// </summary>
        public static T? FechamentoParaSaldoInicial<T>(IReadOnlyList<T> itemsAnoAnterior, Func<T, string?> perApur)
        {
            if (itemsAnoAnterior.Count == 0)
                return default;

            if (EhTrimestral(itemsAnoAnterior.Select(perApur)))
                return itemsAnoAnterior.FirstOrDefault(x => perApur(x) == "T04");

            return itemsAnoAnterior.FirstOrDefault(x => perApur(x) == "A00");
        }
    }
}
