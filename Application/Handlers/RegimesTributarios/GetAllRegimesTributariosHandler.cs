using Application.Queries.RegimesTributarios;
using AutoMapper;
using Domain.Contracts.RegimesTributarios;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegimesTributarios
{

    public class GetAllRegimesTributariosHandler : IRequestHandler<GetAllRegimesTributariosQuery, PagedApiResponse<RegimeTributarioDto>>
    {
        private readonly IBaseRepository<RegimeTributario> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegimesTributariosHandler(IBaseRepository<RegimeTributario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegimeTributarioDto>> Handle(GetAllRegimesTributariosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegimeTributario, RegimeTributarioDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.Ano > 0)
                        q = q.Where(e => e.Ano == request.Filter.Ano);


                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Ano)
                        : q.OrderBy(e => e.Ano);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegimeTributarioDto>>(data)
            );

            return pagedResult;
        }
    }
}
