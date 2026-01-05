using Application.Queries.Tenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Tenants;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Tenants
{
    public class GetAllTenantsHandler : IRequestHandler<GetAllTenantsQuery, PagedApiResponse<TenantDto>>
    {
        private readonly IBaseRepository<Tenant> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllTenantsHandler(IBaseRepository<Tenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<Tenant, TenantDto>(request.Filter, applyFilters: q =>
            {
                if (!string.IsNullOrWhiteSpace(request.Filter.Nome))
                    q = q.Where(e => e.Nome.Contains(request.Filter.Nome.Trim()));

                if (!string.IsNullOrWhiteSpace(request.Filter.Plano))
                    q = q.Where(e => e.Plano.Contains(request.Filter.Plano.Trim()));

                if (!string.IsNullOrWhiteSpace(request.Filter.Slug))
                    q = q.Where(e => e.Slug.Contains(request.Filter.Slug.Trim()));

                q = request.Filter.OrderByDescending
                    ? q.OrderByDescending(e => e.Nome)
                    : q.OrderBy(e => e.Nome);

                return q;
            },
            mapFunc: data => _mapper.Map<IEnumerable<TenantDto>>(data));

            return pagedResult;
        }
    }
}

