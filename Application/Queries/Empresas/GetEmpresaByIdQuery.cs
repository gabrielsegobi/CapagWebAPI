using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Empresas
{
    public class GetEmpresaByIdQuery: IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
