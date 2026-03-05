using Application.Commands.OperationFiles;
using Application.Exceptions.OperationFiles;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.OperationFiles
{
    public class DeleteOperationFileHandler : IRequestHandler<DeleteOperationFileCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<OperationFile> _operationFileRepository;

        public DeleteOperationFileHandler(IBaseRepository<OperationFile> operationFileRepository)
        {
            _operationFileRepository = operationFileRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteOperationFileCommand request, CancellationToken cancellationToken)
        {
            var operationFile = await _operationFileRepository.GetByIdAsync(request.Id) ?? throw new OperationFileNotFoundException(request.Id);

            _operationFileRepository.Delete(operationFile);
            await _operationFileRepository.SaveChangesAsync();

            return new DeleteApiResponse
            {
                Message = $"Arquvio excluído com sucesso."
            };
        }
    }
}
