namespace Application.Filters
{
    public class ICPLimitFilter : BaseFilter
    {
        public string ColorCode { get; set; } = string.Empty;
        public bool? IsActive { get; set; } 
    }
}
