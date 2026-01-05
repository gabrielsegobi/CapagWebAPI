using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Queries.DemonstrativosContabeis
{
    public class GetDClByAnoAndCodigoQuery : IRequest<List<DClByAnoAndCodigoDto>>
    {
        public long IdEmpresa { get; set; }
        public bool Ano { get; set; }
    }
}
