using MediatR;

namespace Application.Commands.Indicadores
{
    public class CalcIndicadoresCommand : IRequest<Dictionary<string, List<object>>>
    {
        public long IdEmpresa { get; set; }
    }
}
