using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Demonstrativos
{
    public class GetBalancoQuery : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
    }
}
