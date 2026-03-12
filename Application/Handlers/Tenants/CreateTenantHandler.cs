using Application.Commands.Tenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Tenants
{
    public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<Tenant> _baseRepository;
        private readonly IMapper _mapper;

        public CreateTenantHandler(IBaseRepository<Tenant> baseRespository, IMapper mapper)
        {
            _baseRepository = baseRespository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = _mapper.Map<Tenant>(request.CreateTenantRequest) ?? throw new InvalidDataException("invalid data");

            await _baseRepository.AddAsync(tenant);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("Tenant Criado com sucesso", tenant.IdTenant);
        }
    }
}
