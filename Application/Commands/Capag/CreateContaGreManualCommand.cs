using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Capag
{
    public class CreateContaGreManualCommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public UpsertContaGreManualRequest Request { get; set; } = new();
    }
}
