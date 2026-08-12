using Domain.Contracts.CapagSimples;
using MediatR;

namespace Application.Queries.CapagSimples
{
    public class GetManualDemonstrativeValuesQuery : IRequest<ManualDemonstrativeValuesResponse>
    {
        public long IdEmpresa { get; set; }
        public List<string>? Kinds { get; set; }
    }
}
