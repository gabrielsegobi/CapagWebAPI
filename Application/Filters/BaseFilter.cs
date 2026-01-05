namespace Application.Filters
{
    public class BaseFilter
    {
        public int? Page { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
        public string? Sort { get; set; }
        public bool OrderByDescending { get; set; } = false;
    }
}