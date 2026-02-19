using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsFileName
{
    public class DeleteRegFileNameCommnad : IRequest<CreateApiResponse>
    {
        public DeleteRegFileNameCommnad(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
