using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Views.UsuariosAcessos
{
    public class GetUAViewByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
