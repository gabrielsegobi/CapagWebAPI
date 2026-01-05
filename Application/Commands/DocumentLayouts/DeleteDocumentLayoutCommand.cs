using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DocumentLayouts
{
    public class DeleteDocumentLayoutCommand : IRequest<DeleteApiResponse>
    {
        public long Id { get; set; }
        public DeleteDocumentLayoutCommand(int id)
        {
            Id = id;
        }
    }
}
