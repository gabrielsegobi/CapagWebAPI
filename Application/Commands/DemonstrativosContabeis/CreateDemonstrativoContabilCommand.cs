using MediatR;

namespace Application.Commands.DemonstrativosContabeis
{
    public class CreateDemonstrativoContabilCommand : IRequest
    {
        public long IdEmpresa { get; set; }
    }
}
