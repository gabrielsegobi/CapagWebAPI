using Domain.Contracts.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.CapagCalculadoraResultados
{
    public class UpdateCapagCalculadoraResultadoCommand : IRequest<UpdateApiResponse>
    {
        public UpdateCapagCalculadoraResultadoCommand(UpdateCapagCalculadoraResultadoRequest request, long id)
        {
            Request = request;
            Id = id;
        }

        public UpdateCapagCalculadoraResultadoRequest Request { get; set; }
        public long Id { get; set; }
    }
}
