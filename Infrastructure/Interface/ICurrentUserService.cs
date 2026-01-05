namespace Infrastructure.Interface
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        long? TenantId { get; }
        bool IsAuthenticated { get; }

        string? GetClaimValue(string claimType);
        IDictionary<string, string> GetAllClaims();
        void SetTenantId(long? tenantId);
    }
}
