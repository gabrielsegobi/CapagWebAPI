using Domain.Contracts.RegimesTributarios;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.RegimesTributarios
{
    public class CreateRegimeTributarioCommand : IRequest<CreateApiResponse>
    {
        public CreateRegimeTributarioRequest CreateRegimeTributarioRequest { get; set; } = new CreateRegimeTributarioRequest();
    }
}