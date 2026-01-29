using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesCalc;
using MediatR;

namespace Application.Commands.SimulacoesCalc
{
    public class CreateSimulacaoCalcCommand : IRequest<CreateApiResponse>
    {
        public CreateSimulacaoCalcCommand(CreateSimulacaoCalcRequest request)
        {
            Request = request;
        }

        public CreateSimulacaoCalcRequest Request { get; set; }
    }
}
