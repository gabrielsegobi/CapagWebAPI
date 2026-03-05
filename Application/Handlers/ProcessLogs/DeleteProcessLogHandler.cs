using Application.Commands.ProcessLog;
using Application.Exceptions.ProcessLogs;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ProcessLogs
{
    public class DeleteProcessLogHandler : IRequestHandler<DeleteProcessLogCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<ProcessLog> _processLogRepository;

        public DeleteProcessLogHandler(IBaseRepository<ProcessLog> processLogRepository)
        {
            _processLogRepository = processLogRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteProcessLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _processLogRepository.GetByIdAsync(request.Id) ?? throw new ProcessLogNotFoundException(request.Id);

            _processLogRepository.Delete(log);
            await _processLogRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Log de processo deletado com sucesso." };
        }
    }
}
