using Microsoft.Extensions.Configuration;

namespace Infrastructure.Helpers
{
    public class RefreshTokenHelper
    {
        private readonly IConfiguration _configuration;

        public RefreshTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DateTime GetExpiration()
        {
            int expireDays = int.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "1");
            return DateTimeHelper.GetDateTimeNow().AddDays(expireDays);
        }
    }
}
