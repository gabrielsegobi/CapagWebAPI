namespace Domain.Contracts.Views
{
    public class DEAViewDto
    {
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public int? Ano { get; set; }
        public decimal? IcpCalculado { get; set; }
        public char? Classificacao { get; set; }
        public decimal? MargemLiquida { get; set; }
        public decimal? Roe { get; set; }
        public decimal? LiquidezCorrente { get; set; }
    }
}
