using Domain.Contracts.CapagE1;
using MediatR;

namespace Application.Queries.CapagE1
{
    public class GetCapagE1CalculoQuery : IRequest<CapagE1CalculoPayload>
    {
        public long IdEmpresa { get; set; }
        public string? Modelo { get; set; }
    }
}
