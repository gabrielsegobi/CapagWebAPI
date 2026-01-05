using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroCommand : IRequest<CreateApiResponse>
    {
        public string FileName { get; }
        public string Type { get; }
        public List<RegDirfTerceiroItemRequest> Requests { get; set; }
        public CreateRegDirfTerceiroCommand(string fileName, string type, List<RegDirfTerceiroItemRequest> requests)
        {
            Requests = requests;
            FileName = fileName;
            Type = type;
        }
    }
}
