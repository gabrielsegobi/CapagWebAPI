using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.AnalisesICP
{
    public class GetAnaliseICPByIdQuery: IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
