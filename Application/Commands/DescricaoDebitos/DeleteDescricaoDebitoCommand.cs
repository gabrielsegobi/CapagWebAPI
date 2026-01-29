using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DescricaoDebitos
{
    public class DeleteDescricaoDebitoCommand : IRequest<DeleteApiResponse>
    {
        public DeleteDescricaoDebitoCommand(long id)
        {
            Id = id;
        }

        public long Id { get; set; }
    }
}
