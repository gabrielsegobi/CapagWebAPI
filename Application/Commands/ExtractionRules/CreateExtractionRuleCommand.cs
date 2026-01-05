using Domain.Contracts.ExtractionRules;
using Domain.Contracts.Responses;
using MediatR;

namespace Application.Commands.ExtractionRules
{
    public class CreateExtractionRuleCommand : IRequest<CreateApiResponse>
    {
        public CreateExtractionRuleRequest CreateExtractionRuleRequest { get; set; }
        public CreateExtractionRuleCommand(CreateExtractionRuleRequest createExtractionRuleRequest)
        {
            CreateExtractionRuleRequest = createExtractionRuleRequest;
        }
    }
}
