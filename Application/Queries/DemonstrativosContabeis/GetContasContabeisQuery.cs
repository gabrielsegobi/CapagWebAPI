using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Queries.DemonstrativosContabeis
{
    public class GetContasContabeisQuery : IRequest<List<ContaContabilDto>>
    {
        public long IdTenant { get; set; }
        public long? IdEmpresa { get; set; }
    }
}
