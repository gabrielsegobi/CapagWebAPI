using Application.Filters;
using Domain.Contracts.RegDefis;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDefis
{
    public class GetAllRegDefisQuery : IRequest<PagedApiResponse<RegDefisDto>>
    {
        public GetAllRegDefisQuery(RegDefisFilter filter)
        {
            Filter = filter;
        }

        public RegDefisFilter Filter { get; set; }
    }
}
