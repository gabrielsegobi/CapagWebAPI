using Domain.Contracts.ProcessLog;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ProcessLog
{
    public class CreateProcessLogCommand: IRequest<CreateApiResponse>
    {
        public CreateProcessLogRequest CreateProcessLogRequest {  get; set; } = new CreateProcessLogRequest();
    }
}
