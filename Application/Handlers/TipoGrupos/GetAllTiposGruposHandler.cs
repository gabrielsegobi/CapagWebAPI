using Application.Queries.TipoGrupos;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.TipoGrupos;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.TipoGrupos
{
    public class GetAllTiposGruposHandler : IRequestHandler<GetAllTiposGruposQuery, PagedApiResponse<TipoGrupoDto>>
    {
        private readonly IBaseRepository<TipoGrupo> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllTiposGruposHandler(IBaseRepository<TipoGrupo> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<TipoGrupoDto>> Handle(GetAllTiposGruposQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<TipoGrupo, TipoGrupoDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Name))
                        q = q.Where(e => e.Name.ToString().Contains(request.Filter.Name.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.Tag))
                        q = q.Where(e => e.Tag.ToString().Contains(request.Filter.Tag.Trim()));

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Name)
                        : q.OrderBy(e => e.Name);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<TipoGrupoDto>>(data)
            );

            return pagedResult;
        }
    }
}
