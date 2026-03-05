using Application.Queries.RegDarf;
using AutoMapper;
using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDarf
{
    public class GetAllRegsDarfHandler : IRequestHandler<GetAllRegsDarfQuery, PagedApiResponse<RegDarfDto>>
    {
        private readonly IBaseRepository<RegDarfs> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegsDarfHandler(IBaseRepository<RegDarfs> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegDarfDto>> Handle(GetAllRegsDarfQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegDarfs, RegDarfDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.IdFilename >= 0)
                        q = q.Where(e => e.IdFilename == request.Filter.IdFilename);

                    if (request.Filter.ValorTotal >= 0)
                        q = q.Where(e => e.ValorTotal == request.Filter.ValorTotal);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.ValorTotal)
                        : q.OrderBy(e => e.ValorTotal);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegDarfDto>>(data)
            );

            return pagedResult;
        }
    }
}
