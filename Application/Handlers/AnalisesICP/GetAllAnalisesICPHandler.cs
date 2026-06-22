using Application.Queries.AnalisesICP;
using AutoMapper;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.AnalisesICP
{
    public class GetAllAnalisesICPHandler : IRequestHandler<GetAllAnalisesICPQuery, PagedApiResponse<AnaliseICPDto>>
    {
        private readonly IBaseRepository<AnaliseICP> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllAnalisesICPHandler(IBaseRepository<AnaliseICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<AnaliseICPDto>> Handle(GetAllAnalisesICPQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query(a => a.DeletedAt == null);

            var pagedResult = await query.ReadPage<AnaliseICP, AnaliseICPDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Classificacao)
                        : q.OrderBy(e => e.Classificacao);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<AnaliseICPDto>>(data)
            );

            return pagedResult;
        }
    }
}
