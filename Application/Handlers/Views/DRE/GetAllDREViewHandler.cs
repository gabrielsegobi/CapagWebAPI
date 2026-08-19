using Application.Interfaces;
using Application.Queries.Views.DRE;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Views.DRE
{
    public class GetAllDReViewHandler : IRequestHandler<GetAllDREViewQuery, PagedApiResponse<DREViewDto>>
    {
        private readonly IBaseViewRepository<DREVw> _viewRepository;
        private readonly IMapper _mapper;
        private readonly INormalizadorSinalService _normalizador;

        public GetAllDReViewHandler(
            IBaseViewRepository<DREVw> viewRepository,
            IMapper mapper,
            INormalizadorSinalService normalizador)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
            _normalizador = normalizador;
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
                        ? q.OrderByDescending(e => e.Codigo)
                        : q.OrderBy(e => e.Codigo);

                    return q;
                },
                mapFunc: data =>
                {
                    var dtos = _mapper.Map<List<DREViewDto>>(data);
                    foreach (var dto in dtos)
                    {
                        dto.ValorNormalizado = _normalizador.Normalizar(dto.Codigo, dto.ValCtaRefFin, dto.IndValCtaRefFin);
                    }
                    return dtos;
                }
            );

            return pagedResult;
        }
    }
}
