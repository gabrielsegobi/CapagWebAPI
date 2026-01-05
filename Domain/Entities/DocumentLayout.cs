namespace Domain.Entities
{
    public class DocumentLayout
    {
        public long Id { get; set; }
        public string LayoutName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ValidationRegex { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public bool? Active { get; set; }

        public ICollection<ExtractionRule> ExtractionRules { get; set; } = new List<ExtractionRule>();
    }
}
