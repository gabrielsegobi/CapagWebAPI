using Application.Commands.ExtractionRules;
using Application.Exceptions.DocumentLayouts;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ExtractionRules
{
    public class CreateExtractionRuleHandler : IRequestHandler<CreateExtractionRuleCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<ExtractionRule> _baseRepository;
        private readonly IBaseRepository<DocumentLayout> _layoutRepository;
        private readonly IMapper _mapper;



        public CreateExtractionRuleHandler(IBaseRepository<ExtractionRule> baseRepository, IBaseRepository<DocumentLayout> layoutRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _layoutRepository = layoutRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateExtractionRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = _mapper.Map<ExtractionRule>(request.CreateExtractionRuleRequest) 
                ?? throw new InvalidDataException("Invalid data");

            var layoutExistente = await _layoutRepository.GetFirstOrDefaultAsync(l => l.Id == request.CreateExtractionRuleRequest.LayoutId);
            if (layoutExistente == null)
                throw new LayoutNotFoundException(rule.LayoutId);

            //var layoutExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.LayoutName == layout.LayoutName);

            //if (layoutExistente != null)
            //    throw new LayoutNameConflictException(layout.LayoutName);



            await _baseRepository.AddAsync(rule);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Regra Criada com Sucesso"
            };
        }
    }
}
