using Application.Filters;
using Domain.Contracts.DescricaoDebitos;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.DescricaoDebitos
{
    public class GetAllDescricoesDebitosQuery : IRequest<PagedApiResponse<DescricaoDebitoDto>>
    {
        public GetAllDescricoesDebitosQuery(DescricaoDebitoFilter filter)
        {
            Filter = filter;
        }

        public DescricaoDebitoFilter Filter { get; set; }
    }
}
