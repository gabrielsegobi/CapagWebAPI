using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDarf
{
    public class CreateRegDarfCommand : IRequest<CreateApiResponse>
    {
        public string FileName { get; }
        public string Type { get; }
        public List<RegDarfItemRequest> Requests { get; set; }
        public CreateRegDarfCommand(string fileName, string type, List<RegDarfItemRequest> requests)
        {
            FileName = fileName;
            Type = type;
            Requests = requests;
        }
    }
}
