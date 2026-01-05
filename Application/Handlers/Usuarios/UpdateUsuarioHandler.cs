using Application.Commands.Usuarios;
using Application.Exceptions.Usuarios;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class UpdateUsuarioHandler : IRequestHandler<UpdateUsuarioCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateUsuarioHandler(IBaseRepository<Usuario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<UpdateApiResponse> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _baseRepository.GetByIdAsync(request.Id);
            if (usuario == null)
            {
                throw new UsuarioNotFoundException(request.Id);
            }
            var usuarioToUpdate = _mapper.Map(request.UpdateUsuarioRequest, usuario);

            _baseRepository.Update(usuarioToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Usuario atualizado com sucesso." };
        }
    }
}
