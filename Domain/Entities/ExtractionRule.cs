namespace Domain.Entities
{
    public class ExtractionRule
    {
        public long Id { get; set; }
        public long LayoutId { get; set; }
        public string FieldLabel { get; set; } = string.Empty;
        public string ExtractionRegex { get; set; } = string.Empty;
        public int? RegexGroupIndex { get; set; }
        public string DestinationTable { get; set; } = string.Empty;
        public string DestinationColumn { get; set; } = string.Empty;
        public string? DataType { get; set; } = string.Empty;

        public DocumentLayout Layout { get; set; }
    }
}
