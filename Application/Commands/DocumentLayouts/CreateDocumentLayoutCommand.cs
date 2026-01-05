using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DocumentLayouts
{
    public class CreateDocumentLayoutCommand : IRequest<CreateApiResponse>
    {
        public CreateDocumentLayoutRequest CreateDocumentLayoutRequest { get; set; }

        public CreateDocumentLayoutCommand(CreateDocumentLayoutRequest createDocumentLayoutRequest)
        {
            CreateDocumentLayoutRequest = createDocumentLayoutRequest;
        }
    }
}
