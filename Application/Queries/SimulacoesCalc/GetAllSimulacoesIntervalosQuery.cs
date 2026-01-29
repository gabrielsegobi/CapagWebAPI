using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesIntervalo;
using MediatR;

namespace Application.Queries.SimulacoesCalc
{
    public class GetAllSimulacoesIntervalosQuery : IRequest<PagedApiResponse<SimulacaoIntervaloDto>>
    {
        public GetAllSimulacoesIntervalosQuery(SimulacaoIntervaloFilter filter)
        {
            Filter = filter;
        }

        public SimulacaoIntervaloFilter Filter { get; set; }
    }
}
