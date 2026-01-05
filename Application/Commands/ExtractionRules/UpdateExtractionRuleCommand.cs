using Domain.Contracts.ExtractionRules;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ExtractionRules
{
    public class UpdateExtractionRuleCommand : IRequest<UpdateApiResponse>
    {
        public long Id { get; set; }
        public UpdateExtractionRuleRequest UpdateExtractionRuleRequest { get; set; }
        public UpdateExtractionRuleCommand(int id, UpdateExtractionRuleRequest updateExtractionRuleRequest)
        {
            Id = id;
            UpdateExtractionRuleRequest = updateExtractionRuleRequest;
        }
    }
}
