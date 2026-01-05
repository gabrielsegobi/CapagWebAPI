using Application.Queries.Views.BalancoPatrimonial;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using MediatR;

namespace Application.Handlers.Views.BalancoPatrimonial
{
    public class GetAllBPViewHandler : IRequestHandler<GetAllBPViewQuery, PagedApiResponse<BPViewDto>>
    {
        private readonly IBaseViewRepository<BalancoPatrimonialVw> _viewRepository;
        private readonly IMapper _mapper;

        public GetAllBPViewHandler(IBaseViewRepository<BalancoPatrimonialVw> viewRepository, IMapper mapper)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<BPViewDto>> Handle(GetAllBPViewQuery request, CancellationToken cancellationToken)
        {
            var query = _viewRepository.Query();

            var pagedResult = await query.ReadPage<BalancoPatrimonialVw, BPViewDto>(
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
                mapFunc: data => _mapper.Map<IEnumerable<BPViewDto>>(data)
            );

            return pagedResult;
        }
    }
}
