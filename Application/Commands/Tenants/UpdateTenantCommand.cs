using Domain.Contracts.Responses;
using Domain.Contracts.Tenants;
using MediatR;

namespace Application.Commands.Tenants
{
    public class UpdateTenantCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; }
        public UpdateTenantRequest UpdateTenantRequest { get; }

        public UpdateTenantCommand(long id, UpdateTenantRequest updateTenantRequest)
        {
            Id = id;
            UpdateTenantRequest = updateTenantRequest;
        }
    }
}
