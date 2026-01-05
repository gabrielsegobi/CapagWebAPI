using Domain.Contracts.Responses;
using MediatR;

namespace Application.Queries.ExtractionRules
{
    public class GetExtractionRuleByIdQuery : IRequest<GetApiResponse>
    {
        public long Id { get; set; }

        public GetExtractionRuleByIdQuery(int id)
        {
            Id = id;
        }
    }
}
