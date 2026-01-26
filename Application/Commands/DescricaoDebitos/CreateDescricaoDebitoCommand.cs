using Domain.Contracts.DescricaoDebitos;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.DescricaoDebitos
{
    public class CreateDescricaoDebitoCommand : IRequest<CreateApiResponse>
    {
        public CreateDescricaoDebitoCommand(CreateDescricaoDebitoRequest request)
        {
            Request = request;
        }

        public CreateDescricaoDebitoRequest Request { get; set; }
    }
}
