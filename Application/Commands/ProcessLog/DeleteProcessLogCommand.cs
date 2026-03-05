using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ProcessLog
{
    public class DeleteProcessLogCommand : IRequest<DeleteApiResponse>
    {
        public DeleteProcessLogCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
