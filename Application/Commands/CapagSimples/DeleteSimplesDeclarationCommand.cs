using MediatR;

namespace Application.Commands.CapagSimples
{
    public class DeleteSimplesDeclarationCommand : IRequest<Unit>
    {
        public long IdEmpresa { get; set; }
    }
}
