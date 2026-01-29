using Domain.Contracts.Responses;
using Domain.Contracts.SimulacoesIntervalo;
using MediatR;

namespace Application.Commands.SimulacoesIntervalo
{
    public class UpdateSimulacaoIntervaloCommand : IRequest<UpdateApiResponse>
    {
        public UpdateSimulacaoIntervaloCommand(long id, UpdateSimulacaoIntervaloRequest request)
        {
            Id = id;
            Request = request;
        }

        public long Id { get; set; }
        public UpdateSimulacaoIntervaloRequest Request { get; set; }
    }
}
