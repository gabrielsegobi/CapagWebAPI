using Domain.Enums;

namespace Domain.Constants
{
    public static class PrlAConstants
    {
        public const string AcaoIncluir = "incluir";
        public const string AcaoIncluirComDesagio = "incluir_com_desagio";
        public const string AcaoExcluir = "excluir";

        public static readonly string[] Acoes =
        [
            AcaoIncluir,
            AcaoIncluirComDesagio,
            AcaoExcluir
        ];

        public static bool IsAcaoValida(string? acao) =>
            !string.IsNullOrWhiteSpace(acao) && Acoes.Contains(acao.Trim().ToLowerInvariant());

        /// <summary>Deságio padrão do bloco de destino, em percentual (0–100).</summary>
        public static decimal DesagioPadrao(BlocoLiquidez bloco) => bloco switch
        {
            BlocoLiquidez.A => 0m,
            BlocoLiquidez.B => 20m,
            BlocoLiquidez.C => 50m,
            _ => 0m
        };

        /// <summary>
        /// Base = saldo_manual ?? saldo_original.
        /// excluir → 0; incluir → base sem deságio; demais → aplica percentual.
        /// </summary>
        public static decimal CalcularSaldoAjustado(
            decimal saldoOriginal,
            decimal? saldoManual,
            decimal percentualDesagio,
            string? acao)
        {
            if (acao == AcaoExcluir)
                return 0m;

            var baseValor = saldoManual ?? saldoOriginal;
            if (acao == AcaoIncluir)
                return Math.Round(baseValor, 2, MidpointRounding.AwayFromZero);

            return Math.Round(baseValor * (1m - percentualDesagio / 100m), 2, MidpointRounding.AwayFromZero);
        }
    }
}
