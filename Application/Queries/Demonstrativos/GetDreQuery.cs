using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Demonstrativos
{
    public class GetDreQuery : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
    }
}
