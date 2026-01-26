using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDarf
{
    public class CreateRegDarfCommand : IRequest<CreateApiResponse>
    {
        public CreateRegDarfCommand(CreateRegDarfRequest requests)
        {
            Requests = requests;
        }

        public CreateRegDarfRequest Requests { get; set; }
    }
}

