using Domain.Contracts.CapagSimples;
using MediatR;

namespace Application.Commands.CapagSimples
{
    public class UpsertManualDemonstrativeValuesCommand : IRequest<ManualDemonstrativeValuesResponse>
    {
        public long IdEmpresa { get; set; }
        public UpsertManualDemonstrativeValuesRequest Request { get; set; } = null!;
    }
}
