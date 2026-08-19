using Domain.Contracts.PrlA;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.PrlA
{
    public class PatchDesagioPrlACommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public PatchDesagioPrlARequest Request { get; set; } = new();
    }
}
