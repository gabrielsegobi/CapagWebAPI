using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.CodigosRegistroDescricao
{
    public class GetCodigoRegistroDescricaoByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
