using Application.Filters;
using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsPgdasd
{
    public class GetAllRegsPgdasdQuery : IRequest<PagedApiResponse<RegPgdasdDto>>
    {
        public RegPgdasdFilter Filter { get; }
        public GetAllRegsPgdasdQuery(RegPgdasdFilter filter)
        {
            Filter = filter;
        }
    }
}
