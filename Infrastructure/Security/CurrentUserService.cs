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
        private long? _tenantId;
        private string? _role;

        public bool IsAuthenticated =>
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        private long? _manualTenantId;
        public long? UserId => GetClaimValue(ClaimTypes.NameIdentifier)?.ToLong();
        public string? Email => GetClaimValue(ClaimTypes.Email);
        public string? Role => _role;
        public long? TenantId => _tenantId;

        //public long? TenantId
        //{
        //    get
        //    {
        //        if (_manualTenantId.HasValue)
        //            return _manualTenantId;

        //        var tenantClaim = GetClaimValue("tenantId");
        //        return tenantClaim?.ToLong();
        //    }
        //}


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
            _tenantId = tenantId;
        }

        //public void SetRole(string? role)
        //{
        //    if (string.IsNullOrWhiteSpace(role))
        //        return;

        //    var httpContext = _httpContextAccessor.HttpContext;
        //    if (httpContext == null)
        //        return;

        //    var identity = httpContext.User.Identity as ClaimsIdentity;
        //    if (identity == null)
        //        return;

        //    var hasRole = identity.Claims.Any(c =>
        //        c.Type == ClaimTypes.Role && c.Value == role);

        //    if (hasRole)
        //        return;

        //    identity.AddClaim(new Claim(ClaimTypes.Role, role));
        //}

        public void SetRole(string role)
        {
            _role = role;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
                return;

            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            if (identity.HasClaim(ClaimTypes.Role, role))
                return;

            identity.AddClaim(new Claim(ClaimTypes.Role, role));
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
