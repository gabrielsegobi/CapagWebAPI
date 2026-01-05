using Application.Exceptions.DocumentLayouts;
using Application.Queries.DocumentLayouts;
using AutoMapper;
using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DocumentLayouts
{
    public class GetDocumentLayoutByIdHandler : IRequestHandler<GetDocumentLayoutByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;
        private readonly IMapper _mapper;

        public GetDocumentLayoutByIdHandler(IBaseRepository<DocumentLayout> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetDocumentLayoutByIdQuery request, CancellationToken cancellationToken)
        {
            //var layout = await _baseRepository.GetByIdAsync(request.Id) ?? throw new DocumentLayoutNotFoundException(request.Id);
            var layout = await _baseRepository.Query().Include(x => x.ExtractionRules).FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new DocumentLayoutNotFoundException(request.Id);
            var result = _mapper.Map<DocumentLayoutDto>(layout);

            return new GetApiResponse { Data = result, Message = "Layout Encontrado com sucesso" };
        }
    }
}
