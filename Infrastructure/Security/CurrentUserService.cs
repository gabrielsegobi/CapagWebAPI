using Infrastructure.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        private long? _manualTenantId;
        public long? UserId => GetClaimValue(ClaimTypes.NameIdentifier)?.ToLong();
        public string? Email => GetClaimValue(ClaimTypes.Email);
        public string? Role => GetClaimValue(ClaimTypes.Role);
        public long? TenantId
        {
            get
            {
                if (_manualTenantId.HasValue)
                    return _manualTenantId;

                var tenantClaim = GetClaimValue("tenantId");
                return tenantClaim?.ToLong();
            }
        }


        public string? GetClaimValue(string claimType)
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?
                .FirstOrDefault(c => c.Type == claimType)?.Value;
        }

        public IDictionary<string, string> GetAllClaims()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims?
                .ToDictionary(c => c.Type, c => c.Value) ?? new Dictionary<string, string>();
        }

        public void SetTenantId(long? tenantId)
        {
            _manualTenantId = tenantId;
        }
    }

    internal static class ClaimExtensions
    {
        public static long? ToLong(this string? value)
        {
            return long.TryParse(value, out var result) ? result : null;
        }
    }
}
