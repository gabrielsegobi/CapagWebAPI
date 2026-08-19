using Domain.Enums;

namespace Application.Services.Demonstrativos
{
    public class ContaAnoSaldo
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public byte Nivel { get; set; }
        public char Tipo { get; set; }
        public int Ano { get; set; }
        public decimal SaldoBruto { get; set; }
        public char? Indicador { get; set; }
        public decimal SaldoNormalizado { get; set; }
        public decimal? SaldoInicialBruto { get; set; }
        public char? IndicadorInicial { get; set; }
        public decimal? SaldoInicialNormalizado { get; set; }
        public GrupoContabil Grupo { get; set; }
        public bool IsDre { get; set; }
    }
}
