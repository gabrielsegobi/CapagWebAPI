using Application.Filters;
using Application.Filters.Views;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using MediatR;

namespace Application.Queries.Views.BalancoPatrimonial
{
    public class GetAllBPViewQuery : IRequest<PagedApiResponse<BPViewDto>>
    {
        public BPViewFilter Filter { get; set; }

        public GetAllBPViewQuery(BPViewFilter filter)
        {
            Filter = filter;
        }
    }
}
