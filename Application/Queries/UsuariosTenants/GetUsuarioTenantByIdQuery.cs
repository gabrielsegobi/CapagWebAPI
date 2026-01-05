using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.UsuariosTenants
{
    public class GetUsuarioTenantByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }

        public GetUsuarioTenantByIdQuery(long id)
        {
            Id = id;
        }
    }
}
