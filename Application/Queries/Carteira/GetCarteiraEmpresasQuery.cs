using Application.Filters;
using Domain.Contracts.Carteira;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Carteira
{
    public class GetCarteiraEmpresasQuery : IRequest<PagedApiResponse<CarteiraEmpresaDto>>
    {
        public CarteiraEmpresaFilter Filter { get; set; }

        public GetCarteiraEmpresasQuery(CarteiraEmpresaFilter filter)
        {
            Filter = filter;
        }
    }
}
