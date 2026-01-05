using Domain.Contracts.Responses;
using Domain.Contracts.UsuarioTenant;
using MediatR;

namespace Application.Commands.UsuariosTenants
{
    public class CreateUsuarioTenantCommand : IRequest<CreateApiResponse>
    {
        public CreateUsuarioTenantRequest CreateUsuarioTenantRequest = new CreateUsuarioTenantRequest();

        public CreateUsuarioTenantCommand(CreateUsuarioTenantRequest request)
        {
            CreateUsuarioTenantRequest = request;
        }
    }
}
