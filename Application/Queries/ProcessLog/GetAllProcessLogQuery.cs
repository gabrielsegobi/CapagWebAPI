using Application.Filters;
using Domain.Contracts.ProcessLog;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ProcessLog
{
    public class GetAllProcessLogQuery(ProcessLogFilter filter) : IRequest<PagedApiResponse<ProcessLogDto>>
    {
        public ProcessLogFilter Filter { get; set; } = filter;
    }
}
 