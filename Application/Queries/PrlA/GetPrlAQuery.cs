using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.PrlA
{
    public class GetPrlAQuery : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public int? Ano { get; set; }
    }
}
