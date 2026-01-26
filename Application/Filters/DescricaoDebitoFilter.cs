namespace Application.Filters
{
    public class DescricaoDebitoFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }
        public string Natureza { get; set; } = string.Empty;
        public string NumCda { get; set; } = string.Empty;
        public int? DataInscricao { get; set; }
        public decimal? ValorPrincipal { get; set; }
        public decimal? ValorMulta { get; set; }
        public decimal? ValorJuros { get; set; }
        public decimal? ValorEncargos { get; set; }

    }
}
