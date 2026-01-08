namespace Application.Filters
{
    public class AnaliseIcpRelatorioFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }
        public decimal? IcpCalculado { get; set; }
        public string? Classificacao { get; set; } = string.Empty;
        public decimal? SomaPesos { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
