using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using MediatR;

namespace Application.Commands.Usuarios
{
    public class CreateUsuarioCommand: IRequest<CreateApiResponse>
    {
        public CreateUsuarioRequest CreateUsuarioRequest { get; set; } = new CreateUsuarioRequest();
    }
}
