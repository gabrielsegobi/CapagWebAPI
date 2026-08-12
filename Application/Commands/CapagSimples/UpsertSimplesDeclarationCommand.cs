using Domain.Contracts.CapagSimples;
using MediatR;

namespace Application.Commands.CapagSimples
{
    public class UpsertSimplesDeclarationCommand : IRequest<SimplesDeclarationResponse>
    {
        public long IdEmpresa { get; set; }
        public UpsertSimplesDeclarationRequest Request { get; set; } = null!;
    }
}
