using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Empresas
{
    public class DeleteEmpresaCommand: IRequest<DeleteApiResponse>
    {
        public long IdEmpresa { get; set; }
    }
}
