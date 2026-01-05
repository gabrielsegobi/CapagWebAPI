using Domain.Contracts.Responses;
using MediatR;
using ZstdSharp.Unsafe;

namespace Application.Commands.ExtractionRules
{
    public class DeleteExtractionRuleCommand : IRequest<DeleteApiResponse>
    {
        public long Id { get; set; }
        public DeleteExtractionRuleCommand(int id)
        {
            Id = id;
        }
    }
}
