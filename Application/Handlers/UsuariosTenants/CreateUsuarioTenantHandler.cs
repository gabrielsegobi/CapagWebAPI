using Application.Commands.UsuariosTenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.UsuariosTenants
{
    public class CreateUsuarioTenantHandler : IRequestHandler<CreateUsuarioTenantCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<UsuarioTenant> _baseRepository;
        private readonly IMapper _mapper;

        public CreateUsuarioTenantHandler(IBaseRepository<UsuarioTenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<CreateApiResponse> Handle(CreateUsuarioTenantCommand request, CancellationToken cancellationToken)
        {
            var usuariotenant = _mapper.Map<UsuarioTenant>(request.CreateUsuarioTenantRequest);

            if (usuariotenant == null)
            {
                throw new InvalidDataException("Invalid Data");
            }

            await _baseRepository.AddAsync(usuariotenant);
            await _baseRepository.SaveChangesAsync(); 

           return new CreateApiResponse { Message = "Usuário tenant adicionado com sucesso" };
        }
    }
}
