using MediatR;

namespace Application.Commands.ResultadosIndicesICP
{
    public class CalcResultadosIndicesICPCommand : IRequest<Dictionary<string, double>>
    {
        public long IdEmpresa { get; set; }
    }
}
