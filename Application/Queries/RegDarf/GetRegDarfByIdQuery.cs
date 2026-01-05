using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.RegDarf
{
    public class GetRegDarfByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
        public GetRegDarfByIdQuery(long id)
        {
            Id = id;
        }
    }
}
