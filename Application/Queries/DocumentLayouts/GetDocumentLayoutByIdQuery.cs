using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.DocumentLayouts
{
    public class GetDocumentLayoutByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }

        public GetDocumentLayoutByIdQuery(int id)
        {
            Id = id;
        }
    }
}
