using Domain.Contracts.AnalisesICP;
using MediatR;

namespace Application.Queries.AnalisesICP
{
    public class GetAnaliseIcpRelatorioQuery
        : IRequest<List<AnaliseIcpRelatorioDto>>
    {
    }
}
