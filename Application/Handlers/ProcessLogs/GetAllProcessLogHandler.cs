using Application.Queries.ProcessLog;
using AutoMapper;
using Domain.Contracts.ProcessLog;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ProcessLogs
{
    public class GetAllProcessLogHandler : IRequestHandler<GetAllProcessLogQuery, PagedApiResponse<ProcessLogDto>>
    {
        private readonly IBaseRepository<ProcessLog> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllProcessLogHandler(IBaseRepository<ProcessLog> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<ProcessLogDto>> Handle(GetAllProcessLogQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<ProcessLog, ProcessLogDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Acao))
                        q = q.Where(e => e.Acao.ToString().Contains(request.Filter.Acao.Trim()));

                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdTenant > 0)
                        q = q.Where(e => e.IdTenant == request.Filter.IdTenant);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Acao)
                        : q.OrderBy(e => e.Acao);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<ProcessLogDto>>(data)
            );

            return pagedResult;
        }
    }
}
