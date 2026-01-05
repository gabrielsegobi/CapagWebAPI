namespace Application.Filters
{
    public class TenantFilter : BaseFilter
    {
        public string Nome { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Plano { get; set; } = string.Empty;
    }
}
