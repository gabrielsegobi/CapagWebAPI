using Application.Exceptions.UsuariosTenant;
using Application.Queries.UsuariosTenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.UsuarioTenant;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.UsuariosTenants
{
    public class GetUsuarioTenantByIdHandler : IRequestHandler<GetUsuarioTenantByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<UsuarioTenant> _baseRepository;
        private readonly IMapper _mapper;

        public GetUsuarioTenantByIdHandler(IBaseRepository<UsuarioTenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetUsuarioTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var usuariotenant = await _baseRepository.GetByIdAsync(request.Id);

            if (usuariotenant == null)
            {
                throw new UsuarioTenantNotFoundException(request.Id);
            }

            var result = _mapper.Map<UsuarioTenantDto>(usuariotenant);

            return new GetApiResponse { Data = result, Message = "Usuário tenant encontrado com sucesso" };
        }
    }
}
