using Domain.Contracts.Empresas;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Empresas
{
    public class UpdateEmpresaImpedimentoCommand : IRequest<UpdateApiResponse>
    {
        public long EmpresaId { get; set; }
        public UpdateEmpresaImpedimentoRequest UpdateEmpresaImpedimentoRequest { get; set; } = new UpdateEmpresaImpedimentoRequest();
    }
}
