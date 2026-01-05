using Domain.Contracts.RegsDctf;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsDctf
{
    public class CreateRegDctfCommand : IRequest<CreateApiResponse>
    {
        public string FileName { get; }
        public string Type { get; }
        public List<RegDctfItemRequest> Requests { get; set; }

        public CreateRegDctfCommand (string fileName, string type, List<RegDctfItemRequest> requests)
        {
            Requests = requests;
            FileName = fileName;
            Type = type;
        }
    }
}
