using Application.Commands.DocumentLayouts;
using Application.Exceptions.DocumentLayouts;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DocumentLayouts
{
    public class DeleteDocumentLayoutHandler : IRequestHandler<DeleteDocumentLayoutCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;

        public DeleteDocumentLayoutHandler(IBaseRepository<DocumentLayout> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteDocumentLayoutCommand request, CancellationToken cancellationToken)
        {
            var document = await _baseRepository.GetByIdAsync(request.Id) ??
                throw new DocumentLayoutNotFoundException(request.Id);

            _baseRepository.Delete(document);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse
            {
                Message = $"Layout deletado com sucesso."
            };
        }
    }
}
