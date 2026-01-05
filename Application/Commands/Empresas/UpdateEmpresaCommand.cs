using Domain.Contracts.Empresas;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Empresas
{
    public class UpdateEmpresaCommand : IRequest<UpdateApiResponse>
    {
        public long EmpresaId { get; set; }
        public UpdateEmpresaRequest UpdateEmpresaRequest { get; set; } = new UpdateEmpresaRequest();
    }
}
