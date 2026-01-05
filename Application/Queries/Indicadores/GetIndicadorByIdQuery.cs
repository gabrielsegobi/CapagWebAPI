using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Indicadores
{
    public class GetIndicadorByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
