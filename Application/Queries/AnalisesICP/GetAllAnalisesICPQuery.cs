using Application.Filters;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.AnalisesICP
{
    public class GetAllAnalisesICPQuery : IRequest<PagedApiResponse<AnaliseICPDto>>
    {
        public AnaliseICPFilter Filter { get; set; }
        public GetAllAnalisesICPQuery(AnaliseICPFilter filter)
        {
            Filter = filter;
        }
    }
}
