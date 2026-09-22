using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Capag
{
    public class DeleteContaGreManualCommand : IRequest<GetApiResponse>
    {
        public long EmpresaId { get; set; }
        public long Id { get; set; }
    }
}
