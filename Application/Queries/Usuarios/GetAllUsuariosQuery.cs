using Application.Filters;
using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using MediatR;

namespace Application.Queries.Usuarios
{
    public class GetAllUsuariosQuery : IRequest<PagedApiResponse<UsuarioDto>>
    {
        public UsuarioFilter Filter { get; set; }

        public GetAllUsuariosQuery(UsuarioFilter filter)
        {
            Filter = filter;
        }
    }
}
