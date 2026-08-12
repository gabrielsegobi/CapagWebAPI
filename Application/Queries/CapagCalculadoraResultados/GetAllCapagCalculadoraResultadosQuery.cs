using Application.Filters;
using Domain.Contracts.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.CapagCalculadoraResultados
{
    public class GetAllCapagCalculadoraResultadosQuery : IRequest<PagedApiResponse<CapagCalculadoraResultadoDto>>
    {
        public GetAllCapagCalculadoraResultadosQuery(CapagCalculadoraResultadoFilter filter)
        {
            Filter = filter;
        }

        public CapagCalculadoraResultadoFilter Filter { get; set; }
    }
}
