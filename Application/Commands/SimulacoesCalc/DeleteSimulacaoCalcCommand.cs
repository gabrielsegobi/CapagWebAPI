using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.SimulacoesCalc
{
    public class DeleteSimulacaoCalcCommand : IRequest<DeleteApiResponse>
    {
        public DeleteSimulacaoCalcCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
