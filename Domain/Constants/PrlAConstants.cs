using Domain.Enums;

namespace Domain.Constants
{
    public static class PrlAConstants
    {
        /// <summary>Deságio padrão do bloco de destino, em percentual (0–100).</summary>
        public static decimal DesagioPadrao(BlocoLiquidez bloco) => bloco switch
        {
            BlocoLiquidez.A => 0m,
            BlocoLiquidez.B => 20m,
            BlocoLiquidez.C => 50m,
            _ => 0m
        };
    }
}
