using Application.Commands.ExtractionRules;
using Application.Exceptions.ExtractionRules;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ExtractionRules
{
    public class DeleteExtractionRuleHandler : IRequestHandler<DeleteExtractionRuleCommand, DeleteApiResponse>
    {
        private readonly IBaseRepository<ExtractionRule> _baseRepository;

        public DeleteExtractionRuleHandler(IBaseRepository<ExtractionRule> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<DeleteApiResponse> Handle(DeleteExtractionRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _baseRepository.GetByIdAsync(request.Id) ?? throw new ExtractionRuleNotFoundException(request.Id);


            _baseRepository.Delete(rule);
            await _baseRepository.SaveChangesAsync();

            return new DeleteApiResponse { Message = "Regra deletada com sucesso" };
        }
    }
}
