using Application.Filters;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.AnalisesICP
{
    public class GetAnaliseIcpRelatorioQuery
        : IRequest<PagedApiResponse<AnaliseIcpRelatorioDto>>
    {
        public GetAnaliseIcpRelatorioQuery(AnaliseIcpRelatorioFilter filter)
        {
            Filter = filter;
        }

        public AnaliseIcpRelatorioFilter Filter { get; set; }
    }
}
