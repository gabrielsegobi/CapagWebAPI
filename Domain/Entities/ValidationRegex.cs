namespace Domain.Entities
{
    public class ValidationRegex
    {
        public long Id { get; set; }
        public string Regex { get; set; } = string.Empty;
        public long DocumentLayoutId { get; set; }

        public DocumentLayout Layout { get; set; }
    }
}
