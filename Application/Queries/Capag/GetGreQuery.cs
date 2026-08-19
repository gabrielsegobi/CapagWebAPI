using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Capag
{
    public class GetGreQuery : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
    }
}
