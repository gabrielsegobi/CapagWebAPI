using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroCommand : IRequest<CreateApiResponse>
    {
        public CreateRegDirfTerceiroRequest Requests { get; set; }
        public CreateRegDirfTerceiroCommand(CreateRegDirfTerceiroRequest requests)
        {
            Requests = requests;
        }
    }
}
