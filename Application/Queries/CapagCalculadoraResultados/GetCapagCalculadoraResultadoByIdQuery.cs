using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.CapagCalculadoraResultados
{
    public class GetCapagCalculadoraResultadoByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
