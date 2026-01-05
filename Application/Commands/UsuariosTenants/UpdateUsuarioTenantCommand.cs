using Domain.Contracts.Responses;
using Domain.Contracts.UsuarioTenant;
using MediatR;
using System.Diagnostics.Eventing.Reader;

namespace Application.Commands.UsuariosTenants
{
    public class UpdateUsuarioTenantCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateUsuarioTenantRequest UpdateUsuarioTenantRequest { get; set; } = new UpdateUsuarioTenantRequest();

        public UpdateUsuarioTenantCommand(long id, UpdateUsuarioTenantRequest request)
        {
            Id = id;
            UpdateUsuarioTenantRequest = request;
        }
    }
}
