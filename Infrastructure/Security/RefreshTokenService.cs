using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Infrastructure.Security
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;
        private readonly IConfiguration _configuration;


        public RefreshTokenService(IBaseRepository<RefreshToken> refreshTokenRepository, IConfiguration configuration)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _configuration = configuration;
        }

        public async Task<RefreshToken> CriarRefreshTokenAsync(long idUsuario, string deviceId)
        {
            var refreshToken = new RefreshToken
            {
                IdUsuario = idUsuario,
                DeviceId = deviceId,
                Token = GerarTokenAleatorio(),
                ExpiraEm = DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:RefreshTokenExpireDays"] ?? "7")),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return refreshToken;
        }

        public async Task RevogarAsync(string token)
        {
            var rt = await _refreshTokenRepository.GetFirstOrDefaultAsync(x => x.Token == token);
            if (rt != null)
            {
                rt.RevogadoEm = DateTime.UtcNow;
                _refreshTokenRepository.Update(rt);
                await _refreshTokenRepository.SaveChangesAsync();
            }
        }

        private static string GerarTokenAleatorio()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
