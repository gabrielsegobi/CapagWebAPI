using Domain.Contracts.RegIrpf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsIrpf
{
    public class CreateRegIrpfCommand : IRequest<CreateApiResponse>
    {
        public string FileName { get; }
        public string Type { get; }
        public List<RegIrpfItemRequest> Requests { get; set; }
        public CreateRegIrpfCommand(
           string fileName,
           string type,
           List<RegIrpfItemRequest> requests)
        {
            FileName = fileName;
            Type = type;
            Requests = requests;
        }
    }
}
