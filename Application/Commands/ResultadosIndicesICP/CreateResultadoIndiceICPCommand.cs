using Domain.Contracts.AnalisesICP;
using Domain.Contracts.ResultadosIndicesICP;
using MediatR;

namespace Application.Commands.ResultadosIndicesICP
{
    public class CreateResultadoIndiceICPCommand : IRequest
    {
        public List<CreateResultadoIndiceICPRequest> CreateResultadosIndicesICPRequest = new();
    }
}
