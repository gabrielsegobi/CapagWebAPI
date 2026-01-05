using Application.Commands.DocumentLayouts;
using Application.Exceptions.DocumentLayouts;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DocumentLayouts
{
    public class UpdateDocumentLayoutHandler : IRequestHandler<UpdateDocumentLayoutCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;
        private readonly IMapper _mapper;
        public UpdateDocumentLayoutHandler(IBaseRepository<DocumentLayout> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<UpdateApiResponse> Handle(UpdateDocumentLayoutCommand request, CancellationToken cancellationToken)
        {
            var layout =  await _baseRepository.GetByIdAsync(request.Id) ?? throw new DocumentLayoutNotFoundException(request.Id);

            var layoutExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.LayoutName == layout.LayoutName);

            if (layoutExistente != null)
                throw new LayoutNameConflictException(layout.LayoutName);

            var layoutToUpdate = _mapper.Map(request.UpdateDocumentLayoutRequest, layout);

            _baseRepository.Update(layoutToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Layout atualizado com sucesso." };
        }
    }
}
