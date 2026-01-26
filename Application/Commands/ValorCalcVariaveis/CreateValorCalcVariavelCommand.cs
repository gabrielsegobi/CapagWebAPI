using Domain.Contracts.Responses;
using Domain.Contracts.ValorCalcVariaveis;
using MediatR;

namespace Application.Commands.ValorCalcVariaveis
{
    public class CreateValorCalcVariavelCommand : IRequest<CreateApiResponse>
    {
        public CreateValorCalcVariavelCommand(CreateValorCalcVariavelRequest request)
        {
            Request = request;
        }

        public CreateValorCalcVariavelRequest Request { get; set; }
    }
}
