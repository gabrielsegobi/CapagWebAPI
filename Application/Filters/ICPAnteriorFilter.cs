namespace Application.Filters
{
    public class ICPAnteriorFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }
        public char? Classificacao { get; set; }
        public decimal? ValorICPReceita { get; set; }
    }
}
