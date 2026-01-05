using Application.Filters;
using Domain.Contracts.RegimesTributarios;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegimesTributarios
{
    public class GetAllRegimesTributariosQuery : IRequest<PagedApiResponse<RegimeTributarioDto>>
    {
        public RegimeTributarioFilter Filter { get; set; }

        public GetAllRegimesTributariosQuery(RegimeTributarioFilter filter)
        {
            Filter = filter;
        }
    }
}
