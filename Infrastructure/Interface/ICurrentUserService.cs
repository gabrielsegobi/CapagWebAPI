namespace Infrastructure.Interface
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        string? Email { get; }
        string? Role { get; }
        long? TenantId { get; }
        bool IsAuthenticated { get; }

        /// <summary>Usuário Gmaster em modo demonstração (header X-Gmaster-Is-Demo).</summary>
        bool IsDemo { get; }

        /// <summary>Id do usuário no Gmaster (header X-Gmaster-User-Id), se informado.</summary>
        string? GmasterUserId { get; }

        /// <summary>CNPJs raiz (8 dígitos) permitidos para usuário demo (header X-Gmaster-Demo-Cnpjs).</summary>
        IReadOnlySet<string> DemoCnpjRoots { get; }

        string? GetClaimValue(string claimType);
        IDictionary<string, string> GetAllClaims();
        void SetTenantId(long? tenantId);
        void SetRole(string? role);
        void SetDemoContext(bool isDemo, string? gmasterUserId, IEnumerable<string>? demoCnpjRoots);
    }
}
