using Application.Filters;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Indicadores
{
    public class GetAllIndicadoresQuery: IRequest<PagedApiResponse<IndicadorDto>>
    {
        public IndicadorFilter Filter { get; set; }

        public GetAllIndicadoresQuery(IndicadorFilter filter)
        {
            Filter = filter;
        }
    }
}
