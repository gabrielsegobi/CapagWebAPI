using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesCalc;
using MediatR;

namespace Application.Commands.SimulacoesCalc
{
    public class UpdateSimulacaoCalcCommand : IRequest<UpdateApiResponse>
    {
        public UpdateSimulacaoCalcCommand(long id, UpdateSimulacaoCalcRequest request)
        {
            Id = id;
            Request = request;
        }

        public long Id { get; set; }

        public UpdateSimulacaoCalcRequest Request { get; set; }
    }
}
