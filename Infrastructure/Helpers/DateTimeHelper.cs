namespace Infrastructure.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetDateTimeNow()
        {
            var now = DateTime.UtcNow;
            return now.AddHours(-3);
        }
    }
}
