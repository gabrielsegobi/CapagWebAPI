using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Tenants
{
    public class DeleteTenantCommand : IRequest<DeleteApiResponse>
    {
        public long Id { get; }

        public DeleteTenantCommand(long id)
        {
            Id = id;
        }
    }
}
