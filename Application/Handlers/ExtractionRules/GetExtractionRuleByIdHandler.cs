using Application.Exceptions.ExtractionRules;
using Application.Queries.ExtractionRules;
using AutoMapper;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ExtractionRules
{
    public class GetExtractionRuleByIdHandler : IRequestHandler<GetExtractionRuleByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<ExtractionRule> _baseRepository;
        private readonly IMapper _mapper;

        public GetExtractionRuleByIdHandler(IBaseRepository<ExtractionRule> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetExtractionRuleByIdQuery request, CancellationToken cancellationToken = default)
        {
            var rule = await _baseRepository.GetByIdAsync(request.Id) ?? throw new ExtractionRuleNotFoundException(request.Id);

            var result = _mapper.Map<ExtractionRuleDto>(rule);

            return new GetApiResponse { Data = result, Message = "Regra Encontada com sucesso" };
        }
    }
}
