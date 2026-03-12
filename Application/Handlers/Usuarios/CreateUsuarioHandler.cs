using Application.Commands.Usuarios;
using Application.Exceptions.Usuarios;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class CreateUsuarioHandler : IRequestHandler<CreateUsuarioCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public CreateUsuarioHandler(IBaseRepository<Usuario> baseRepository, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<CreateApiResponse> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = _mapper.Map<Usuario>(request.CreateUsuarioRequest);
            if (usuario == null)
            {
                throw new InvalidDataException("Invalid data");
            }

            var usuarioExistente = await _baseRepository.GetFirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (usuarioExistente != null)
            {
                throw new UsuarioEmailConflictException(usuario.Email);
            }

            usuario.SenhaHash = _passwordHasher.Hash(request.CreateUsuarioRequest!.SenhaHash);

            await _baseRepository.AddAsync(usuario);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("Usuário Criado com Sucesso", usuario.IdUsuario);
        }
    }
}
