using Application.Filters;
using Domain.Contracts.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.CodigosRegistroDescricao
{
    public class GetAllCodigosRegistroDescricaoQuery : IRequest<PagedApiResponse<CodigoRegistroDescricaoDto>>
    {
        public GetAllCodigosRegistroDescricaoQuery(CodigoRegistroDescricaoFilter filter)
        {
            Filter = filter;
        }

        public CodigoRegistroDescricaoFilter Filter { get; set; }
    }
}
