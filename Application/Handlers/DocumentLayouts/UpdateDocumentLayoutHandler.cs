using Application.Commands.DocumentLayouts;
using Application.Exceptions.DocumentLayouts;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DocumentLayouts
{
    public class UpdateDocumentLayoutHandler : IRequestHandler<UpdateDocumentLayoutCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;
        private readonly IBaseRepository<ValidationRegex> _validationRegexRepository;
        private readonly IMapper _mapper;

        public static string UpdateMessage = "Layout atualizado com sucesso.";
        public UpdateDocumentLayoutHandler(IBaseRepository<DocumentLayout> baseRepository, IMapper mapper, IBaseRepository<ValidationRegex> validationRegexRepository)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _validationRegexRepository = validationRegexRepository;
        }
        public async Task<UpdateApiResponse> Handle(UpdateDocumentLayoutCommand request, CancellationToken cancellationToken)
        {
            var layout =  await _baseRepository.Query().Include(x => x.ValidationRegexes).FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new DocumentLayoutNotFoundException(request.Id);

            var layoutExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.LayoutName == layout.LayoutName && e.Id != request.Id);

            if (layoutExistente != null)
                throw new LayoutNameConflictException(layout.LayoutName);

            _mapper.Map(request.UpdateDocumentLayoutRequest, layout);


            if (request.UpdateDocumentLayoutRequest.ValidationRegexes != null)
            {
                var requestRegexes = request.UpdateDocumentLayoutRequest.ValidationRegexes;

                var requestIds = requestRegexes
                    .Where(x => x.Id > 0)
                     .Select(x => x.Id)
                    .ToList();

                var regexesParaRemover = layout.ValidationRegexes
                    .Where(db => !requestIds.Contains(db.Id))
                    .ToList();

                foreach (var regex in regexesParaRemover)
                {
                    layout.ValidationRegexes.Remove(regex);
                    _validationRegexRepository.Delete(regex);
                }

                foreach (var regexRequest in requestRegexes)
                {
                    if (regexRequest.Id == 0)
                    {
                        var novaRegex = _mapper.Map<ValidationRegex>(regexRequest);
                        layout.ValidationRegexes.Add(novaRegex);
                    }
                    else
                    {
                        var regexExistente = layout.ValidationRegexes
                                 .FirstOrDefault(x => x.Id == regexRequest.Id);

                        if (regexExistente != null)
                        {
                            _mapper.Map(regexRequest, regexExistente);
                        }
                    }
                }
            }
            _baseRepository.Update(layout);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = UpdateMessage };
        }
    }
}
