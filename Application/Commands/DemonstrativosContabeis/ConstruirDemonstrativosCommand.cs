using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Commands.DemonstrativosContabeis
{
    public class ConstruirDemonstrativosCommand : IRequest<ConstruirDemonstrativosResponse>
    {
        public ConstruirDemonstrativosRequest Request { get; }

        public ConstruirDemonstrativosCommand(ConstruirDemonstrativosRequest request)
        {
            Request = request;
        }
    }
}

