using Domain.Contracts.PrlA;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.PrlA
{
    public class PatchSaldoPrlACommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public PatchSaldoPrlARequest Request { get; set; } = new();
    }
}
