using Domain.Contracts.Responses;
using Domain.Contracts.TipoGrupos;
using MediatR;

namespace Application.Queries.TipoGrupos
{
    public class GetVariaveisQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
        public GetVariaveisRequest Request { get; set; }
        public GetVariaveisQuery(long id, GetVariaveisRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
