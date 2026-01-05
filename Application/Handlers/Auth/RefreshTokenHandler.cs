using Application.Commands.Auth;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using Infrastructure.Security;
using MediatR;

namespace Application.Handlers.Auth
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;
        private readonly IBaseRepository<Usuario> _usuarioRepository;
        private readonly IBaseRepository<UsuarioTenant> _usuarioTenantRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(
            IBaseRepository<RefreshToken> refreshTokenRepository,
            IBaseRepository<Usuario> usuarioRepository,
            IBaseRepository<UsuarioTenant> usuarioTenantRepository,
            ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioTenantRepository = usuarioTenantRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var token = request.Token;
            var deviceId = request.DeviceId ?? "unknown";

            var refreshToken = await _refreshTokenRepository.GetFirstOrDefaultAsync(rt =>
                rt.Token == token &&
                rt.DeviceId == deviceId &&
                rt.RevogadoEm == null &&
                rt.ExpiraEm > DateTimeHelper.GetDateTimeNow());

            if (refreshToken == null)
                return new LoginResponse
                {
                    Token = "a",
                    RefreshToken = request.Token,
                    Nome ="igonara",
                    Email = "igonara",
                    Papel = "igonara",
                };
            //throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");

            var usuario = await _usuarioRepository.GetFirstOrDefaultAsync(u => u.IdUsuario == refreshToken.IdUsuario);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuário não encontrado.");

            var usuarioTenant = await _usuarioTenantRepository.GetFirstOrDefaultAsync(ut =>
                ut.IdUsuario == usuario.IdUsuario &&
                ut.Ativo &&
                ut.DeletedAt == null);

            var papel = usuarioTenant?.Papel.ToString() ?? "visualizador";

            var accessToken = _tokenService.GerarToken(usuario, papel);

            refreshToken.RevogadoEm = DateTimeHelper.GetDateTimeNow();
            _refreshTokenRepository.Update(refreshToken);

            var novoRefreshToken = new RefreshToken
            {
                IdUsuario = usuario.IdUsuario,
                DeviceId = deviceId,
                Token = RandomGenerator.Generate(),
                ExpiraEm = DateTimeHelper.GetDateTimeNow().AddDays(7),
                CreatedAt = DateTimeHelper.GetDateTimeNow(),
                UpdatedAt = DateTimeHelper.GetDateTimeNow()
            };

            await _refreshTokenRepository.AddAsync(novoRefreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new LoginResponse
            {
                Token = accessToken,
                RefreshToken = novoRefreshToken.Token,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = papel
            };



        }

    }
}
