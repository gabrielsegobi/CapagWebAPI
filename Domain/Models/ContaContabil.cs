using Domain.Enums;

namespace Domain.Models
{
    /// <summary>
    /// Conta com saldo bruto e natureza próprios. O sinal econômico é sempre
    /// derivado do <see cref="IndicadorDC"/> desta conta, nunca do grupo-pai na árvore.
    /// </summary>
    public class ContaContabil
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public IndicadorDC IndicadorDC { get; set; }
        public GrupoContabil Grupo { get; set; }
        public decimal SaldoBruto { get; set; }
    }
}
