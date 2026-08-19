using Domain.Constants;

namespace Application.Helpers
{
    /// <summary>
    /// Fórmulas usam <c>2.03</c>/<c>2.03.*</c>; no balanço o PL pode estar como <c>3</c>/<c>3.*</c>.
    /// A conta <c>3</c> da DRE (resultado líquido) não entra nesse alias.
    /// </summary>
    public static class PatrimonioLiquidoCodigoMapper
    {
        public static bool EhCodigoFormula(string? codigo)
        {
            var n = SaldoContabilHelper.NormalizarCodigo(codigo);
            return n == PatrimonioLiquidoConstants.CodigoFormula
                || n.StartsWith(PatrimonioLiquidoConstants.CodigoFormula + ".", StringComparison.Ordinal);
        }

        public static bool EhCodigoBalanco(string? codigo)
        {
            var n = SaldoContabilHelper.NormalizarCodigo(codigo);
            return n == PatrimonioLiquidoConstants.CodigoBalanco
                || n.StartsWith(PatrimonioLiquidoConstants.CodigoBalanco + ".", StringComparison.Ordinal);
        }

        /// <summary>
        /// <c>3</c> → <c>2.03</c>, <c>3.04</c> → <c>2.03.04</c>, <c>3.04[I]</c> → <c>2.03.04[I]</c>.
        /// </summary>
        public static string ParaCodigoFormula(string codigoBalanco)
        {
            var abertura = codigoBalanco.EndsWith("[I]", StringComparison.Ordinal) ? "[I]" : string.Empty;
            var n = SaldoContabilHelper.NormalizarCodigo(codigoBalanco);
            if (n == PatrimonioLiquidoConstants.CodigoBalanco)
                return PatrimonioLiquidoConstants.CodigoFormula + abertura;
            if (n.StartsWith(PatrimonioLiquidoConstants.CodigoBalanco + ".", StringComparison.Ordinal))
                return PatrimonioLiquidoConstants.CodigoFormula + n[PatrimonioLiquidoConstants.CodigoBalanco.Length..] + abertura;
            return codigoBalanco;
        }
    }
}
