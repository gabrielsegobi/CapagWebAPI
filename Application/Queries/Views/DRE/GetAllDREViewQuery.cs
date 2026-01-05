using Application.Filters.Views;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using MediatR;

namespace Application.Queries.Views.DRE
{
    public class GetAllDREViewQuery : IRequest<PagedApiResponse<DREViewDto>>
    {
        public DREViewFilter Filter { get; set; }
        public GetAllDREViewQuery(DREViewFilter filter)
        {
            Filter = filter;
        }
    }
}
