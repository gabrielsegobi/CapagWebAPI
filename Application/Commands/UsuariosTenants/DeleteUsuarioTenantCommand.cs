using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.UsuariosTenants
{
    public class DeleteUsuarioTenantCommand : IRequest<DeleteApiResponse>
    {
        public long Id { get; set; }

        public DeleteUsuarioTenantCommand(long id)
        {
            Id = id;
        }
    }
}
