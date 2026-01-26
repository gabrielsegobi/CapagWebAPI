using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.ValorCalcVariaveis;
using MediatR;

namespace Application.Queries.ValorCalcVariaveis
{
    public class GetAllValorCalcVariavelQuery : IRequest<PagedApiResponse<ValorCalcVariavelDto>>
    {
        public GetAllValorCalcVariavelQuery(ValorCalcVariavelFilter filter)
        {
            Filter = filter;
        }

        public ValorCalcVariavelFilter Filter { get; set; }
    }
}