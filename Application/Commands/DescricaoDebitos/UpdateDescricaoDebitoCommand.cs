using Domain.Contracts.DescricaoDebitos;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DescricaoDebitos
{
    public class UpdateDescricaoDebitoCommand : IRequest<UpdateApiResponse>
    {
        public UpdateDescricaoDebitoCommand(UpdateDescricaoDebitoRequest request, long id)
        {
            Request = request;
            Id = id;
        }

        public UpdateDescricaoDebitoRequest Request { get; set; }
        public long Id { get; set; }
    }
}
