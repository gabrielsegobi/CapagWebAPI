using Application.Exceptions.Tenants;
using Application.Queries.Tenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Tenants;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Tenants
{
    public class GetTenantByIdHandler : IRequestHandler<GetTenantByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Tenant> _baseRepository;
        private readonly IMapper _mapper;
        public GetTenantByIdHandler(IBaseRepository<Tenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _baseRepository.GetByIdAsync(request.Id);

            if (tenant == null)
            {
                throw new TenantNotFoundException(request.Id);
            }

            var result = _mapper.Map<TenantDto>(tenant);
            return new GetApiResponse
            {
                Data = result,
                Message = "Tenant encontrado com sucesso"
            };
        }
    }
}
