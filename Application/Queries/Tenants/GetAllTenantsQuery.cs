using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.Tenants;
using MediatR;

namespace Application.Queries.Tenants
{
    public class GetAllTenantsQuery : IRequest<PagedApiResponse<TenantDto>>
    {
        public TenantFilter Filter { get; set; }

        public GetAllTenantsQuery(TenantFilter filter) { Filter = filter; }
    }
}



