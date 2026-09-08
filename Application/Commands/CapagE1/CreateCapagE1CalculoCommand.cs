using Domain.Contracts.CapagE1;
using MediatR;

namespace Application.Commands.CapagE1
{
    public class CreateCapagE1CalculoCommand : IRequest<CapagE1CalculoPayload>
    {
        public CreateCapagE1CalculoCommand(CapagE1CalculoPayload request)
        {
            Request = request;
        }

        public CapagE1CalculoPayload Request { get; set; }
    }
}
