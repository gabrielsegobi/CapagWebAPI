using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using MediatR;

namespace Application.Commands.Usuarios
{
    public class UpdateUsuarioCommand: IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateUsuarioRequest UpdateUsuarioRequest { get; set; } = new UpdateUsuarioRequest();
    }
}
