using Domain.Contracts.Carteira;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.Carteira
{
    public class AtualizarCarteiraEmpresaCommand : IRequest<UpdateApiResponse>
    {
        public long EmpresaId { get; set; }
        public AtualizarCarteiraEmpresaRequest Request { get; set; } = new();
    }
}
