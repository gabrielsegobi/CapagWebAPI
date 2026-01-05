using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Tenants
{
    public class GetTenantByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
