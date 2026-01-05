using Application.Exceptions;
using Application.Exceptions.Usuarios;
using Application.Queries.Usuarios;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Usuarios
{
    public class GetUsuarioByIdHandler : IRequestHandler<GetUsuarioByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        private readonly IMapper _mapper;
        public GetUsuarioByIdHandler(IBaseRepository<Usuario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _baseRepository.GetByIdAsync(request.Id);

            if (usuario == null)
            {
                throw new UsuarioNotFoundException(request.Id);
            }

            var result = _mapper.Map<UsuarioDto>(usuario);
            return new GetApiResponse
            {
                Data = result,
                Message = "Usuario encontrado com sucesso"
            };
        }
    }
}
