using MediatR;

namespace Application.Queries.DemonstrativosContabeis
{
    public class GetAnosCalculoDemonstrativoQuery : IRequest<IReadOnlyList<int>>
    {
        public long IdEmpresa { get; set; }
    }
}
