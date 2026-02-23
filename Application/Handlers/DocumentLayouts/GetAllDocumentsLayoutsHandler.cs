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
    public class GetAllDocumentsLayoutsHandler : IRequestHandler<GetAllDocumentsLayoutsQuery, PagedApiResponse<DocumentLayoutDto>>
    {
        private readonly IBaseRepository<DocumentLayout> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllDocumentsLayoutsHandler(IBaseRepository<DocumentLayout> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<DocumentLayoutDto>> Handle(GetAllDocumentsLayoutsQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query().Include(er => er.ExtractionRules).Include(vr => vr.ValidationRegexes);

            var pagedResult = await query.ReadPage<DocumentLayout, DocumentLayoutDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.LayoutName))
                        q = q.Where(e => e.LayoutName.ToString().Contains(request.Filter.LayoutName.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.Description))
                        q = q.Where(e => (e.Description ?? string.Empty).Contains(request.Filter.Description.Trim()));

                    if (request.Filter.Active.HasValue)
                        q = q.Where(e => e.Active == request.Filter.Active.Value);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.LayoutName)
                        : q.OrderBy(e => e.LayoutName);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<DocumentLayoutDto>>(data)
            );

            return pagedResult;
        }
    }
}
