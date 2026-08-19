using Domain.Contracts.Carteira;
using MediatR;

namespace Application.Queries.Carteira
{
    public class GetPainelContratosQuery : IRequest<PainelContratosDto> { }
}
