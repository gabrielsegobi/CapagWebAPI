using Domain.Contracts.Usuarios;
using MediatR;

namespace Application.Queries.Usuarios
{
    public class GetUserTenantsQuery : IRequest<List<UserTenantResponse>>
    {
    }
}

