namespace Domain.Entities
{
    public class ICPLimit : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public string? Label { get; set; } = string.Empty;
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public byte? SortOrder { get; set; }
        public string ColorCode { get; set; } = string.Empty;
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
