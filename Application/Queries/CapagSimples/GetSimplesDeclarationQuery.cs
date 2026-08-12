using Domain.Contracts.CapagSimples;
using MediatR;

namespace Application.Queries.CapagSimples
{
    public class GetSimplesDeclarationQuery : IRequest<SimplesDeclarationResponse>
    {
        public long IdEmpresa { get; set; }
    }
}
