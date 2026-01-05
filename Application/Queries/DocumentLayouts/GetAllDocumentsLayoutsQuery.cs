using Application.Filters;
using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.DocumentLayouts
{
    public class GetAllDocumentsLayoutsQuery : IRequest<PagedApiResponse<DocumentLayoutDto>>
    {
        public DocumentLayoutFilter Filter { get; set; }

        public GetAllDocumentsLayoutsQuery(DocumentLayoutFilter filter)
        {
            Filter = filter;
        }
    }
}
