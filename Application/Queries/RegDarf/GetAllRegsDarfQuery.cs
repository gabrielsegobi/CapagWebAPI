using Application.Filters;
using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDarf
{
    public class GetAllRegsDarfQuery : IRequest<PagedApiResponse<RegDarfDto>>
    {
        public RegDarfFilter Filter { get; set; }
        public GetAllRegsDarfQuery(RegDarfFilter filter)
        {
            Filter = filter;
        }
    }
}
