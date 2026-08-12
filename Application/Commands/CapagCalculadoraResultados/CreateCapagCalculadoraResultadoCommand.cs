using Domain.Contracts.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CapagCalculadoraResultados
{
    public class CreateCapagCalculadoraResultadoCommand : IRequest<CreateApiResponse>
    {
        public CreateCapagCalculadoraResultadoCommand(CreateCapagCalculadoraResultadoRequest request)
        {
            Request = request;
        }

        public CreateCapagCalculadoraResultadoRequest Request { get; set; }
    }
}
