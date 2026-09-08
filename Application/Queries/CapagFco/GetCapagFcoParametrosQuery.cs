using Domain.Contracts.CapagFco;
using MediatR;

namespace Application.Queries.CapagFco
{
    public class GetCapagFcoParametrosQuery : IRequest<CapagFcoParametrosResponse>
    {
        public long IdEmpresa { get; set; }
    }
}
