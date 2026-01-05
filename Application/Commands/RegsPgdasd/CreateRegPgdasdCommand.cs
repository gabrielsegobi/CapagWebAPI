using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsPgdasd
{
    public class CreateRegPgdasdCommand : IRequest<CreateApiResponse>
    {
        public string FileName { get; }
        public string Type { get; }
        public List<RegPgdasdItemRequest> Requests { get; set; }
        public CreateRegPgdasdCommand(string fileName, string type, List<RegPgdasdItemRequest> requests)
        {
            FileName = fileName;
            Type = type;
            Requests = requests;
        }
    }
}
