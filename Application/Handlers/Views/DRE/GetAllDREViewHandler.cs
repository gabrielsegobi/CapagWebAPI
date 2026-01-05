using Application.Queries.Views.DRE;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using MediatR;

namespace Application.Handlers.Views.DRE
{
    public class GetAllDReViewHandler : IRequestHandler<GetAllDREViewQuery, PagedApiResponse<DREViewDto>>
    {
        private readonly IBaseViewRepository<DREVw> _viewRepository;
        private readonly IMapper _mapper;

        public GetAllDReViewHandler(IBaseViewRepository<DREVw> viewRepository, IMapper mapper)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<DREViewDto>> Handle(GetAllDREViewQuery request, CancellationToken cancellationToken)
        {
            var query = _viewRepository.Query();

            var pagedResult = await query.ReadPage<DREVw, DREViewDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.Ano > 0)
                        q = q.Where(e => e.Ano == request.Filter.Ano);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Descricao)
                        : q.OrderBy(e => e.Descricao);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<DREViewDto>>(data)
            );

            return pagedResult;
        }
    }
}
