using Domain.Contracts.DemonstrativosContabeis;
using MediatR;

namespace Application.Queries.DemonstrativosContabeis
{
    public class GetAnosDisponiveisQuery : IRequest<AnosDisponiveisResponse>
    {
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
    }
}

