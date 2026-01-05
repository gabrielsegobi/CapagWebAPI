using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using MediatR;

namespace Application.Commands.Usuarios
{
    public class ChangePasswordCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public ChangePasswordRequest ChangePasswordRequest { get; set; } = new ChangePasswordRequest();
    }
}
