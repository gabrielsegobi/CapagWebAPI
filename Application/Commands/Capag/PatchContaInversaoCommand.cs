using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Capag
{
    public class PatchContaInversaoCommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public PatchContaInversaoRequest Request { get; set; } = new();
    }
}
