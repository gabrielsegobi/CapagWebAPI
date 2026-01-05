using Application.Commands.Usuarios;
using Application.Exceptions.Usuarios;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class DeleteUsuarioHandler : IRequestHandler<DeleteUsuarioCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        public DeleteUsuarioHandler(IBaseRepository<Usuario> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _baseRepository.GetByIdAsync(request.Id);

            if (usuario == null)
            {
                throw new UsuarioNotFoundException(request.Id);
            }

            usuario.DeletedAt = DateTimeHelper.GetDateTimeNow();

            _baseRepository.Update(usuario);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Usuario deletado Com Sucesso" };
        }
    }
}
