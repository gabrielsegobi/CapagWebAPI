using Domain.Contracts.RegsDctf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsDctf
{
    public class CreateRegDctfCommand : IRequest<CreateApiResponse>
    {
        public CreateRegDctfCommand(CreateRegDctfRequest requests)
        {
            Requests = requests;
        }

        public CreateRegDctfRequest Requests { get; set; }
    }
}
