using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Usuarios
{
    public class GetUsuarioByIdQuery: IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
