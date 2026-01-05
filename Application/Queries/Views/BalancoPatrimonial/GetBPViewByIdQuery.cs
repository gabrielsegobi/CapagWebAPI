using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.Views.BalancoPatrimonial
{
    public class GetBPViewByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }
    }
}
