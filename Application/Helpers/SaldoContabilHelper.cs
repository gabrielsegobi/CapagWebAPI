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
    }
}
