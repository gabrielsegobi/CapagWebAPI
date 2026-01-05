namespace Application.Filters
{
    public class DocumentLayoutFilter : BaseFilter
    {
        public string LayoutName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? Active { get; set; }
    }
}
