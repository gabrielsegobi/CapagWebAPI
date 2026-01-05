using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ModelosIndicesICP
{
    public class GetModeloIndiceICPByIdQuery:IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
