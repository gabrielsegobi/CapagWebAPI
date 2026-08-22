using Application.Services.SinalContabil;
using Domain.Constants;
using Domain.Entities;

namespace Application.Helpers
{
    public static class SaldoContabilHelper
    {
        /// <summary>
        /// ECF: C = +, D = −. Preferir <see cref="Application.Interfaces.INormalizadorSinalService"/> via DI.
        /// Exceção: conta de custos <c>3.01.01.03</c> (somente ela) sempre em magnitude positiva.
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
        /// Conta de custos <c>3.01.01.03</c> (somente ela): ignora D/C e usa sempre magnitude positiva nos cálculos.
        /// </summary>
        public static bool EhContaCustoSemprePositiva(string? codigo) =>
            NormalizarCodigo(codigo) == DreCapagConstants.CodigoGrupoCustos;

        /// <summary>
        /// Encaminha ao normalizador: C = +, D = − (custos <c>3.01.01.03</c> sempre positivos).
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

        public const string NomePmp = "Prazo Médio de Pagamento (PMP)";
        public const string NomeCoberturaJuros = "Cobertura de Juros (CJ)";

        /// <summary>
        /// PMP e Cobertura de Juros usam magnitude (|valor|), ignorando D/C.
        /// Demais indicadores/ICP usam o sinal C = + / D = −.
        /// </summary>
        public static bool UsaMagnitudeAbsolutaNaFormula(string? nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return false;

            return nome.Equals(NomePmp, StringComparison.OrdinalIgnoreCase)
                || nome.Equals(NomeCoberturaJuros, StringComparison.OrdinalIgnoreCase);
        }

        public static Dictionary<string, double> ComMagnitudeTodas(Dictionary<string, double> valores) =>
            valores.ToDictionary(kv => kv.Key, kv => Math.Abs(kv.Value));

        public static Dictionary<string, double> ValoresParaFormula(
            Dictionary<string, double> valores,
            string? nomeIndicador) =>
            UsaMagnitudeAbsolutaNaFormula(nomeIndicador)
                ? ComMagnitudeTodas(valores)
                : valores;
    }
}
