using Application.Commands.RefreshTokens;
using Application.Exceptions.Usuarios;
using Application.Queries;
using AutoMapper;
using Domain.Contracts.RefreshTokens;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers
{
    public class AuthHandler : IRequestHandler<AuthQuery, LoginResponse>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        private readonly IBaseRepository<UsuarioTenant> _usuarioTenantRepository;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;

        public AuthHandler(
            IBaseRepository<Usuario> baseRepository,
            IBaseRepository<UsuarioTenant> usuarioTenantRepository,
            IMapper mapper,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IMediator mediator)
        {
            _baseRepository = baseRepository;
            _usuarioTenantRepository = usuarioTenantRepository;
            _mapper = mapper;
            _tokenService = tokenService;
            _mediator = mediator;
            _passwordHasher = passwordHasher;
        }
        public async Task<LoginResponse> Handle(AuthQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _baseRepository.GetFirstOrDefaultAsync(u => u.Email == request.Email);

            if (usuario == null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash))
                throw new UsuarioUnauthorizedException();

            usuario.UltimoAcesso = DateTimeHelper.GetDateTimeNow();
            _baseRepository.Update(usuario);
            await _baseRepository.SaveChangesAsync();

            var usuarioTenant = await _usuarioTenantRepository.GetFirstOrDefaultAsync(
                ut => ut.IdUsuario == usuario.IdUsuario && ut.Ativo);

            var papel = usuarioTenant?.Papel.ToString() ?? "User";

            var token = _tokenService.GerarToken(usuario, papel);

            var refreshToken = await _mediator.Send(new CreateRefreshTokenCommand
            {
                CreateRefreshTokenRequest = new CreateRefreshTokenRequest
                {
                    IdUsuario = usuario.IdUsuario,
                    DeviceId = request.DeviceId ?? "unknown"
                }
            }, cancellationToken);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Papel = papel
            };

        }

    }
}
