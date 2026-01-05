using Application.Filters;
using Domain.Contracts.Empresas;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Empresas
{
    public class GetAllEmpresasQuery: IRequest<PagedApiResponse<EmpresaDto>>
    {
        public EmpresaFilter Filter { get; set; }

        public GetAllEmpresasQuery(EmpresaFilter filter)
        {
            Filter = filter;
        }
    }
}
