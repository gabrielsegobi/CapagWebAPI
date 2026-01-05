using Application.Commands.Usuarios;
using Application.Exceptions.Usuarios;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Usuario> _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordHandler(IBaseRepository<Usuario> usuarioRepository, IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UpdateApiResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var req = request.ChangePasswordRequest;
            if (req == null)
                throw new InvalidDataException("Dados inválidos.");

            var usuario = await _usuarioRepository.GetByIdAsync(request.Id);
            if (usuario == null)
                throw new UsuarioNotFoundException(request.Id);

            if (!_passwordHasher.Verify(req.SenhaAtual, usuario.SenhaHash))
                throw new SenhaIncorretaUnauthorizedException();

            usuario.SenhaHash = _passwordHasher.Hash(req.NovaSenha);
            usuario.UpdatedAt = DateTimeHelper.GetDateTimeNow();

            _usuarioRepository.Update(usuario);
            await _usuarioRepository.SaveChangesAsync();

            return new UpdateApiResponse
            {
                Message = "Senha alterada com sucesso."
            };
        }
    }
}
