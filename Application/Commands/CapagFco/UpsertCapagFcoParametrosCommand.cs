using Domain.Contracts.CapagFco;
using MediatR;

namespace Application.Commands.CapagFco
{
    public class UpsertCapagFcoParametrosCommand : IRequest<CapagFcoParametrosResponse>
    {
        public long IdEmpresa { get; set; }
        public UpsertCapagFcoParametrosRequest Request { get; set; } = null!;
    }
}
