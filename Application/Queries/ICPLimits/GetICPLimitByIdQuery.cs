using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ICPLimits
{
    public class GetICPLimitByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
