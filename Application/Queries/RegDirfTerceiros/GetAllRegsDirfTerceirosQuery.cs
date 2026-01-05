using Application.Filters;
using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDirfTerceiros
{
    public class GetAllRegsDirfTerceirosQuery : IRequest<PagedApiResponse<RegDirfTerceiroDto>>
    {
        public RegDirfTerceiroFilter Filter { get; set; }
        public GetAllRegsDirfTerceirosQuery(RegDirfTerceiroFilter filter)
        {
            Filter = filter;
        }
    }
}

