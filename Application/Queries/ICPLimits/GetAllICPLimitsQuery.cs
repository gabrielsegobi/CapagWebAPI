using Application.Filters;
using Domain.Contracts.ICPLimits;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ICPLimits
{
    public class GetAllICPLimitsQuery : IRequest<PagedApiResponse<ICPLimitDto>>
    {
        public ICPLimitFilter Filter { get; set; }

        public GetAllICPLimitsQuery(ICPLimitFilter filter)
        {
            Filter = filter;
        }
    }
}
