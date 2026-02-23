using Application.Commands.DocumentLayouts;
using Application.Exceptions.DocumentLayouts;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DocumentLayouts
{

    public class CreateDocumentLayoutHandler : IRequestHandler<CreateDocumentLayoutCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;
        private readonly IMapper _mapper;

        public static string CreateMessage = "Layout Criado com Sucesso";
        public CreateDocumentLayoutHandler(IBaseRepository<DocumentLayout> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateDocumentLayoutCommand request, CancellationToken cancellationToken)
        {
            var layout = _mapper.Map<DocumentLayout>(request.CreateDocumentLayoutRequest) ?? throw new InvalidDataException("Invalid data");

            var layoutExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.LayoutName == layout.LayoutName);

            if (layoutExistente != null)
                throw new LayoutNameConflictException(layout.LayoutName);


            await _baseRepository.AddAsync(layout);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = CreateMessage
            };
        }
    }
}
