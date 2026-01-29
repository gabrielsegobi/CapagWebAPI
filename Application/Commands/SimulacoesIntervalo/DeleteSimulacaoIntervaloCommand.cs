using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.SimulacoesIntervalo
{
    public class DeleteSimulacaoIntervaloCommand : IRequest<DeleteApiResponse>
    {
        public DeleteSimulacaoIntervaloCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
