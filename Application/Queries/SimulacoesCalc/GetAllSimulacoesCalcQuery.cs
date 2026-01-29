using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesCalc;
using MediatR;

namespace Application.Queries.SimulacoesCalc
{
    public class GetAllSimulacoesCalcQuery : IRequest<PagedApiResponse<SimulacaoCalcDto>>
    {
        public GetAllSimulacoesCalcQuery(SimulacaoCalcFilter filter)
        {
            Filter = filter;
        }

        public SimulacaoCalcFilter Filter { get; set; }
    }
}
