using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DocumentLayouts
{
    public class UpdateDocumentLayoutCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateDocumentLayoutRequest UpdateDocumentLayoutRequest { get; set; }
        public UpdateDocumentLayoutCommand(long id, UpdateDocumentLayoutRequest updateDocumentLayoutRequest)
        {
            Id = id;
            UpdateDocumentLayoutRequest = updateDocumentLayoutRequest;
        }
    }
}
