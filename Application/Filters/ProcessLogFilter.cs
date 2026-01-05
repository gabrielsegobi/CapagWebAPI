namespace Application.Filters
{
    public class ProcessLogFilter : BaseFilter
    {
        public long IdEmpresa { get; set; }
        public long IdTenant { get; set; }
        public string Acao { get; set; } = string.Empty;
    }
}
