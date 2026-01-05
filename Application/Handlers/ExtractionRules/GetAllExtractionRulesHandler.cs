using Application.Queries.ExtractionRules;
using AutoMapper;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ExtractionRules
{
    public class GetAllExtractionRulesHandler : IRequestHandler<GetAllExtractionRulesQuery, PagedApiResponse<ExtractionRuleDto>>
    {
        private readonly IBaseRepository<ExtractionRule> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllExtractionRulesHandler(IBaseRepository<ExtractionRule> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<ExtractionRuleDto>> Handle(GetAllExtractionRulesQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<ExtractionRule, ExtractionRuleDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.FieldLabel))
                        q = q.Where(e => e.FieldLabel.ToString().Contains(request.Filter.FieldLabel.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.DestinationColumn))
                        q = q.Where(e => e.DestinationColumn.ToString().Contains(request.Filter.DestinationColumn.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.DestinationTable))
                        q = q.Where(e => e.DestinationTable.ToString().Contains(request.Filter.DestinationTable.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.DataType))
                        q = q.Where(e => (e.DataType ?? string.Empty).Contains(request.Filter.DataType.Trim()));

                    if (request.Filter.LayoutId >= 0)
                        q = q.Where(e => e.LayoutId == request.Filter.LayoutId);

                    if (request.Filter.RegexGroupIndex >= 0)
                        q = q.Where(e => e.RegexGroupIndex == request.Filter.RegexGroupIndex);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.LayoutId)
                        : q.OrderBy(e => e.LayoutId);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<ExtractionRuleDto>>(data)
            );

            return pagedResult;
        }
    }
}
