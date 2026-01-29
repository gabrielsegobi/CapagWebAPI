using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesIntervalo;
using MediatR;

namespace Application.Commands.SimulacoesIntervalo
{
    public class CreateSimulacaoIntervaloCommand : IRequest<CreateApiResponse>
    {
        public CreateSimulacaoIntervaloCommand(CreateSimulacaoIntervaloRequest request)
        {
            Request = request;
        }

        public CreateSimulacaoIntervaloRequest Request { get; set; }
    }
}
