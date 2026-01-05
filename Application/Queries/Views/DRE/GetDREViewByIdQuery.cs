using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Views.DRE
{
    public class GetDREViewByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
