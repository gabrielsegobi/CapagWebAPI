using Domain.Contracts.ValoresAnuais;
using MediatR;

namespace Application.Commands.ValoresAnuais
{
    public class CreateValorAnulCommand: IRequest
    {
        public CreateValorAnualRequest CreateValorAnualRequest  = new CreateValorAnualRequest();
    }
}
