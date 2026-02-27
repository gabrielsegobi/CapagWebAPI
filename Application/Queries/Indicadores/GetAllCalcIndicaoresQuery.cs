using Application.Filters;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Indicadores
{
    public class GetAllCalcIndicaoresQuery : IRequest<PagedApiResponse<CalcIndicadoresDto>>
    {
        public GetAllCalcIndicaoresQuery(long idEmpresa, CalcIndicadoresFilter filter)
        {
            IdEmpresa = idEmpresa;
            Filter = filter;
        }

        public long IdEmpresa { get; set; }
        public CalcIndicadoresFilter Filter { get; set; }
    }
}
