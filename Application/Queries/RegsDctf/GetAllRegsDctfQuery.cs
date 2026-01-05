using Application.Filters;
using Domain.Contracts.RegsDctf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegsDctf
{
    public class GetAllRegsDctfQuery : IRequest<PagedApiResponse<RegDctfDto>>
    {
        public RegDctfFilter Filter { get; set; }
        public GetAllRegsDctfQuery(RegDctfFilter filter)
        {
            Filter = filter;
        }
    }
}
