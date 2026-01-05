namespace Application.Filters
{
    public class ExtractionRuleFilter : BaseFilter
    {
        public long? LayoutId { get; set; }
        public string FieldLabel { get; set; } = string.Empty;
        public int? RegexGroupIndex { get; set; }
        public string DestinationTable { get; set; } = string.Empty;
        public string DestinationColumn { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
    }
}
