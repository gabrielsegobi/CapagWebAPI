using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Usuarios
{
    public  class DeleteUsuarioCommand: IRequest<DeleteApiResponse>
    {
        public long Id { get; set; }
    }
}
