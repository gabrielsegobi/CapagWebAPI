using Application.Queries.RegDirfTerceiros;
using AutoMapper;
using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDirfTerceiros
{
    public class GetAllRegsDirfsTerceirosHandler : IRequestHandler<GetAllRegsDirfTerceirosQuery, PagedApiResponse<RegDirfTerceiroDto>>
    {
        private readonly IBaseRepository<RegDirfTerceiro> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllRegsDirfsTerceirosHandler(IBaseRepository<RegDirfTerceiro> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<RegDirfTerceiroDto>> Handle(GetAllRegsDirfTerceirosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<RegDirfTerceiro, RegDirfTerceiroDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Codigo))
                        q = q.Where(e => e.Codigo.ToString().Contains(request.Filter.Codigo.Trim()));

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.IdEmpresa)
                        : q.OrderBy(e => e.IdEmpresa);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<RegDirfTerceiroDto>>(data)
            );

            return pagedResult;
        }
    }
}
