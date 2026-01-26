namespace Application.Filters
{
    public class RegDefisFilter : BaseFilter
    {

        public long? IdEmpresa { get; set; }
        public DateTime? Periodo { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal? Valor { get; set; }
        public long? IdFilename { get; set; }

    }
}
