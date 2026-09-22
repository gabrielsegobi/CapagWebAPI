using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Capag
{
    public class UpdateContaGreManualCommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public long Id { get; set; }
        public UpsertContaGreManualRequest Request { get; set; } = new();
    }
}
