using Domain.Entities;

namespace Infrastructure.Interface
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CriarRefreshTokenAsync(long idUsuario, string deviceId);
        Task RevogarAsync(string token);
    }
}
