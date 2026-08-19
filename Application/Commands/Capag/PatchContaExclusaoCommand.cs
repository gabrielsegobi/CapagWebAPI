using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Capag
{
    public class PatchContaExclusaoCommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public PatchContaExclusaoRequest Request { get; set; } = new();
    }
}
