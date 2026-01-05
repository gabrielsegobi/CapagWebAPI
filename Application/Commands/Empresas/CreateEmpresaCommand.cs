using Domain.Contracts.Empresas;
using Domain.Contracts.Responses;
using MediatR;
namespace Application.Commands.Empresas
{
    public class CreateEmpresaCommand : IRequest<CreateApiResponse>
    {
        public CreateEmpresaRequest CreateEmpresaRequest { get; set; } = new CreateEmpresaRequest();
    }
}
