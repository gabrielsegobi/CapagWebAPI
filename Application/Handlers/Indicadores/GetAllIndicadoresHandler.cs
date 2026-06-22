using Application.Queries.Indicadores;
using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Resources;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Indicadores
{
    public class GetAllIndicadoresHandler : IRequestHandler<GetAllIndicadoresQuery, PagedApiResponse<IndicadorDto>>
    {
        private readonly IBaseRepository<Indicador> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllIndicadoresHandler(IBaseRepository<Indicador> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<IndicadorDto>> Handle(GetAllIndicadoresQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository
                .Query(i => i.DeletedAt == null)
                .Include(i => i.ValoresAnuais);

            var pagedResult = await query.ReadPage<Indicador, IndicadorDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Nome))
                        q = q.Where(e => e.Nome.Contains(request.Filter.Nome.Trim()));

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Nome)
                        : q.OrderBy(e => e.Nome);

                    return q;
                },
                postFilter: items =>
                {
                    foreach (var item in items)
                    {
                        var grupo = IndicadorGrupoMapper.ObterGrupo(item.Nome);
                        item.GetType().GetProperty("Grupo")?.SetValue(item, grupo);
                    }

                    if (!string.IsNullOrWhiteSpace(request.Filter.Grupo))
                    {
                        var grupoFiltro = request.Filter.Grupo.Trim().ToLower();
                        items = items.Where(i =>
                            IndicadorGrupoMapper.ObterGrupo(i.Nome).ToLower() == grupoFiltro
                        );
                    }

                    return items;
                },
                mapFunc: data =>
                {
                    var dtos = _mapper.Map<IEnumerable<IndicadorDto>>(data);
                    foreach (var dto in dtos)
                        dto.Grupo = IndicadorGrupoMapper.ObterGrupo(dto.Nome);

                    return dtos;
                }
            );

            return pagedResult;
        }
    }
}
