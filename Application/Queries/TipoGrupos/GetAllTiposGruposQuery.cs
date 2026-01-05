using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.TipoGrupos;
using MediatR;

namespace Application.Queries.TipoGrupos
{
    public class GetAllTiposGruposQuery : IRequest<PagedApiResponse<TipoGrupoDto>>
    {
        public TipoGrupoFilter Filter { get; set; }
        public GetAllTiposGruposQuery(TipoGrupoFilter filter)
        {
            Filter = filter;
        }
    }
}
