using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.UsuarioTenant;
using MediatR;

namespace Application.Queries.UsuariosTenants
{
    public class GetAllUsuariosTenantQuery : IRequest<PagedApiResponse<UsuarioTenantDto>>
    {
        public UsuarioTenantFilter Filter { get; set; } 

        public GetAllUsuariosTenantQuery(UsuarioTenantFilter filter)
        {
            Filter = filter;
        }
    }
}
