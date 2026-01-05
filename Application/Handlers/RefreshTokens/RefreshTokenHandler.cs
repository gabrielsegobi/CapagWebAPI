//using Application.Commands.Auth;
//using Application.Commands.RefreshTokens;
//using AutoMapper;
//using Domain.Contracts.Responses;
//using Domain.Entities;
//using Infrastructure.Interface;
//using MediatR;

//namespace Application.Handlers.Auth
//{
//    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
//    {
//        private readonly IBaseRepository<Usuario> _usuarioRepository;
//        private readonly IBaseRepository<RefreshToken> _refreshRepository;
//        private readonly ITokenService _tokenService;

//        public RefreshTokenHandler(
//            IBaseRepository<Usuario> usuarioRepository,
//            IBaseRepository<RefreshToken> refreshRepository,
//            ITokenService tokenService)
//        {
//            _usuarioRepository = usuarioRepository;
//            _refreshRepository = refreshRepository;
//            _tokenService = tokenService;
//        }

//        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
//        {
//            var refreshToken = await _refreshRepository.GetFirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.DeviceId == request.DeviceId);

//            if (refreshToken == null || refreshToken.EstaExpirado || refreshToken.EstaRevogado)
//                throw new UnauthorizedAccessException("Refresh token inválido ou expirado.");

//            var usuario = await _usuarioRepository.GetByIdAsync(refreshToken.IdUsuario)
//                ?? throw new UnauthorizedAccessException("Usuário não encontrado.");

//            var token = _tokenService.GerarToken(usuario, "User");

//            return new LoginResponse
//            {
//                Token = token,
//                RefreshToken = refreshToken.Token,
//                Nome = usuario.Nome,
//                Email = usuario.Email
//            };
//        }
//    }
//}
