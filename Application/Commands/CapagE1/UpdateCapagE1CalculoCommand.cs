using Domain.Contracts.CapagE1;
using MediatR;

namespace Application.Commands.CapagE1
{
    public class UpdateCapagE1CalculoCommand : IRequest<CapagE1CalculoPayload>
    {
        public UpdateCapagE1CalculoCommand(CapagE1CalculoPayload request, long id)
        {
            Request = request;
            Id = id;
        }

        public long Id { get; set; }
        public CapagE1CalculoPayload Request { get; set; }
    }
}
