using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Empresas
{
    public class ReprocessarEmpresaCommand : IRequest<UpdateApiResponse>
    {
        public long IdEmpresa { get; set; }
    }
}
