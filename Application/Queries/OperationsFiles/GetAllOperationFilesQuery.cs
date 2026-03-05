using Application.Filters;
using Domain.Contracts.OperationFiles;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.OperationsFiles
{
    public class GetAllOperationFilesQuery : IRequest<PagedApiResponse<OperationFilesDto>>
    {
        public GetAllOperationFilesQuery(OperationFilesFilter filter)
        {
            Filter = filter;
        }

        public OperationFilesFilter Filter { get; set; }
    }
}
