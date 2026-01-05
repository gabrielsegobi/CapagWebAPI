using Domain.Contracts.Responses;
using Domain.Contracts.Tenants;
using MediatR;

namespace Application.Commands.Tenants
{
    public class CreateTenantCommand : IRequest<CreateApiResponse>
    {
        public CreateTenantRequest CreateTenantRequest { get; }

        public CreateTenantCommand(CreateTenantRequest createTenantRequest)
        {
            CreateTenantRequest = createTenantRequest;
        }
    }
}
