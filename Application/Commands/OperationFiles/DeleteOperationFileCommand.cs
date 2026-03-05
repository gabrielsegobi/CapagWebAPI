using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.OperationFiles
{
    public class DeleteOperationFileCommand : IRequest<DeleteApiResponse>
    {
        public DeleteOperationFileCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
