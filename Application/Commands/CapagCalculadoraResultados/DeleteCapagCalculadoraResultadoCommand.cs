using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CapagCalculadoraResultados
{
    public class DeleteCapagCalculadoraResultadoCommand : IRequest<DeleteApiResponse>
    {
        public DeleteCapagCalculadoraResultadoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
