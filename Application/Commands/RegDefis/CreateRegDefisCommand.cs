using Domain.Contracts.RegDefis;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegDefis
{
    public class CreateRegDefisCommand : IRequest<CreateApiResponse>
    {
        public CreateRegDefisCommand(CreateRegDefisRequest request)
        {
            Request = request;
        }

        public CreateRegDefisRequest Request { get; set; }
    }
}
