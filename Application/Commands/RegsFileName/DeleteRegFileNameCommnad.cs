using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsFileName
{
    public class DeleteRegFileNameCommnad : IRequest<DeleteApiResponse>
    {
        public DeleteRegFileNameCommnad(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
