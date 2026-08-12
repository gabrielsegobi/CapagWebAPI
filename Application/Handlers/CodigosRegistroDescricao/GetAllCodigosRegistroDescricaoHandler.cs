using Application.Queries.CodigosRegistroDescricao;
using AutoMapper;
using Domain.Contracts.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CodigosRegistroDescricao
{
    public class GetAllCodigosRegistroDescricaoHandler : IRequestHandler<GetAllCodigosRegistroDescricaoQuery, PagedApiResponse<CodigoRegistroDescricaoDto>>
    {
        private readonly IBaseRepository<CodigoRegistroDescricao> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllCodigosRegistroDescricaoHandler(IBaseRepository<CodigoRegistroDescricao> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<CodigoRegistroDescricaoDto>> Handle(GetAllCodigosRegistroDescricaoQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<CodigoRegistroDescricao, CodigoRegistroDescricaoDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa.HasValue)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Codigo))
                        q = q.Where(e => e.Codigo.Contains(request.Filter.Codigo.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.ExpressaoRegular))
                        q = q.Where(e => e.ExpressaoRegular.Contains(request.Filter.ExpressaoRegular.Trim()));

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "idempresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "codigo" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Codigo)
                            : q.OrderBy(e => e.Codigo),

                        "expressaoregular" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ExpressaoRegular)
                            : q.OrderBy(e => e.ExpressaoRegular),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Id)
                            : q.OrderBy(e => e.Id)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<CodigoRegistroDescricaoDto>>(data)
            );

            return pagedResult;
        }
    }
}
