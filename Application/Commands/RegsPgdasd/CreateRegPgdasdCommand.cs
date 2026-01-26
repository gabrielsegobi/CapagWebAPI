using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegsPgdasd
{
    public class CreateRegPgdasdCommand : IRequest<CreateApiResponse>
    {
        public CreateRegPgdasdCommand(CreateRegPgdasdRequest requests)
        {
            Request = requests;
        }

        public CreateRegPgdasdRequest Request { get; set; }
    }
}
