namespace Application.Filters
{
    public class CodigoRegistroDescricaoFilter : BaseFilter
    {
        public long? IdEmpresa { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string ExpressaoRegular { get; set; } = string.Empty;
        public bool? IsValid { get; set; }
    }
}
