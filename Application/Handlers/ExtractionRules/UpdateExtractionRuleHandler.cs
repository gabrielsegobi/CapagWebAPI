using Application.Commands.ExtractionRules;
using Application.Exceptions.ExtractionRules;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ExtractionRules
{
    public class UpdateExtractionRuleHandler : IRequestHandler<UpdateExtractionRuleCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<ExtractionRule> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateExtractionRuleHandler(IBaseRepository<ExtractionRule> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateExtractionRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _baseRepository.GetByIdAsync(request.Id) ?? throw new ExtractionRuleNotFoundException(request.Id);

            //var layoutExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.LayoutName == layout.LayoutName);

            //if (layoutExistente != null)
            //    throw new LayoutNameConflictException(layout.LayoutName);

            var ruleToUpdate = _mapper.Map(request.UpdateExtractionRuleRequest, rule);

            _baseRepository.Update(ruleToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Regra atualizada com sucesso." };
        }
    }
}
