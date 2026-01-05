using Application.Filters;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ExtractionRules
{
    public class GetAllExtractionRulesQuery : IRequest<PagedApiResponse<ExtractionRuleDto>>
    {
        public ExtractionRuleFilter Filter { get; set; }

        public GetAllExtractionRulesQuery(ExtractionRuleFilter filter)
        {
            Filter = filter;
        }
    }
}
