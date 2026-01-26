using Domain.Contracts.RegIrpf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsIrpf
{
    public class CreateRegIrpfCommand : IRequest<CreateApiResponse>
    {
        public CreateRegIrpfCommand(CreateRegIrpfRequest requests)
        {
            Requests = requests;
        }

        public CreateRegIrpfRequest Requests { get; set; }
    }
}
