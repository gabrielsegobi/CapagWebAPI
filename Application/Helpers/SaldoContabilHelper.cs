using Application.Services.SinalContabil;
using Domain.Entities;

namespace Application.Helpers
{
    public static class SaldoContabilHelper
    {
        /// <summary>
        /// ECF: C = +, D = −. Preferir <see cref="Application.Interfaces.INormalizadorSinalService"/> via DI.
        /// </summary>
        public static decimal SaldoAssinado(decimal? valor, char? indicador, string? codigo = null)
        {
            if (!valor.HasValue)
                return 0m;

            return NormalizadorSinalLocator.Instance.Normalizar(codigo, valor, indicador);
        }

        public static decimal SaldoAssinado(DemonstrativoContabil? registro, bool usarInicial = false)
        {
            if (registro == null)
                return 0m;

            return usarInicial
                ? NormalizadorSinalLocator.Instance.Normalizar(registro.Codigo, registro.ValCtaRefIni, registro.IndValCtaRefIni)
                : NormalizadorSinalLocator.Instance.Normalizar(registro.Codigo, registro.ValCtaRefFin, registro.IndValCtaRefFin);
        }

        /// <summary>
        /// Magnitude positiva do valor contábil, ignorando o indicador ECD (D/C).
        /// </summary>
        public static decimal Magnitude(decimal? valor) =>
            valor.HasValue ? Math.Abs(valor.Value) : 0m;

        public static decimal Magnitude(DemonstrativoContabil? registro, bool usarInicial = false)
        {
            if (registro == null)
                return 0m;

            return usarInicial
                ? Magnitude(registro.ValCtaRefIni)
                : Magnitude(registro.ValCtaRefFin);
        }

        public static string NormalizarCodigo(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            var normalizado = codigo.Trim();
            var idx = normalizado.IndexOf('[');
            return idx >= 0 ? normalizado[..idx] : normalizado;
        }

        public static char? NormalizarIndicador(char? indicador)
        {
            if (!indicador.HasValue)
                return null;

            var c = char.ToUpperInvariant(indicador.Value);
            return c is 'D' or 'C' ? c : indicador;
        }

        public static bool EhDebito(char? indicador) => NormalizarIndicador(indicador) == 'D';

        public static bool EhCredito(char? indicador) => NormalizarIndicador(indicador) == 'C';

        public static string? CodigoPai(string? codigo)
        {
            var n = NormalizarCodigo(codigo);
            var idx = n.LastIndexOf('.');
            return idx <= 0 ? null : n[..idx];
        }

        public static bool EhCodigoOuFilho(string? codigo, string prefixo)
        {
            var n = NormalizarCodigo(codigo);
            return n == prefixo || n.StartsWith(prefixo + ".", StringComparison.Ordinal);
        }

        /// <summary>
        /// Contas de ativo do balanço (<c>1</c> e <c>1.*</c>): D/C da ECF é invertido antes do C = + / D = −.
        /// </summary>
        public static bool EhContaAtivoBalanco(string? codigo)
        {
            var n = NormalizarCodigo(codigo);
            return n == "1" || n.StartsWith("1.", StringComparison.Ordinal);
        }

        /// <summary>
        /// <c>3.01.01</c> e <c>3.01.01.*</c>: nas fórmulas de indicadores e ICP usam magnitude
        /// (ignoram D/C). Na soma da DRE o D/C de cada período entra no cálculo.
        /// </summary>
        public static bool EhCodigoSemSinalDc(string? codigo)
        {
            var n = NormalizarCodigo(codigo);
            return n == "3.01.01" || n.StartsWith("3.01.01.", StringComparison.Ordinal);
        }

        /// <summary>
        /// Encaminha ao normalizador: C = +, D = −.
        /// </summary>
        public static decimal ValorParaFormula(DemonstrativoContabil? registro, string codigo, bool usarInicial = false)
        {
            if (registro == null)
                return 0m;

            if (usarInicial)
                return ValorParaFormula(registro.ValCtaRefIni, registro.IndValCtaRefIni, codigo);

            return ValorParaFormula(registro.ValCtaRefFin, registro.IndValCtaRefFin, codigo);
        }

        public static decimal ValorParaFormula(decimal? valor, char? indicador, string codigo)
        {
            return NormalizadorSinalLocator.Instance.Normalizar(codigo, valor, indicador);
        }

        public static Dictionary<string, double> ComMagnitudeTodas(Dictionary<string, double> valores) =>
            valores.ToDictionary(kv => kv.Key, kv => Math.Abs(kv.Value));

        /// <summary>
        /// Indicadores e ICP: <c>3.01.01</c> e filhos entram em módulo.
        /// </summary>
        public static Dictionary<string, double> ComMagnitude30101(Dictionary<string, double> valores) =>
            valores.ToDictionary(
                kv => kv.Key,
                kv => EhCodigoSemSinalDc(kv.Key) ? Math.Abs(kv.Value) : kv.Value);
    }
}
