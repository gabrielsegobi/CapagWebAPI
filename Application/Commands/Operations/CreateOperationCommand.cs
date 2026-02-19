using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Operations
{
    public class CreateOperationCommand : IRequest<long>
    {
        public CreateOperationCommand(long idEmpresa)
        {
            IdEmpresa = idEmpresa;
        }

        public long IdEmpresa { get; set; }
    }
}
