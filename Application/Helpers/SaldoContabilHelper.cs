using Domain.Entities;

namespace Application.Helpers
{
    public static class SaldoContabilHelper
    {
        /// <summary>Reconstrói saldo assinado a partir do par valor/indicador ECD (D = débito).</summary>
        public static decimal SaldoAssinado(decimal? valor, char? indicador)
        {
            if (!valor.HasValue)
                return 0m;

            var magnitude = Math.Abs(valor.Value);
            return indicador == 'D' ? -magnitude : magnitude;
        }

        public static decimal SaldoAssinado(DemonstrativoContabil? registro, bool usarInicial = false)
        {
            if (registro == null)
                return 0m;

            return usarInicial
                ? SaldoAssinado(registro.ValCtaRefIni, registro.IndValCtaRefIni)
                : SaldoAssinado(registro.ValCtaRefFin, registro.IndValCtaRefFin);
        }

        /// <summary>
        /// Magnitude positiva do valor contábil, ignorando o indicador ECD (D/C).
        /// Usada nas fórmulas de indicadores: o sinal patrimonial não entra no cálculo.
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
    }
}
