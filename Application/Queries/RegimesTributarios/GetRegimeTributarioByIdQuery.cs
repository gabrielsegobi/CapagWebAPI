using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegimesTributarios
{
    public class GetRegimeTributarioByIdQuery: IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
