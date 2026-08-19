using Application.Interfaces;
using Application.Queries.Views.BalancoPatrimonial;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Views.BalancoPatrimonial
{
    public class GetAllBPViewHandler : IRequestHandler<GetAllBPViewQuery, PagedApiResponse<BPViewDto>>
    {
        private readonly IBaseViewRepository<BalancoPatrimonialVw> _viewRepository;
        private readonly IMapper _mapper;
        private readonly INormalizadorSinalService _normalizador;

        public GetAllBPViewHandler(
            IBaseViewRepository<BalancoPatrimonialVw> viewRepository,
            IMapper mapper,
            INormalizadorSinalService normalizador)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
            _normalizador = normalizador;
        }

        public async Task<PagedApiResponse<BPViewDto>> Handle(GetAllBPViewQuery request, CancellationToken cancellationToken)
        {
            var query = _viewRepository.Query();

            // Balanço por empresa costuma trazer dezenas de contas × anos; evita página padrão (10) incompleta.
            if (request.Filter.IdEmpresa > 0 && request.Filter.PageSize is null or 10)
                request.Filter.PageSize = 500;

            var pagedResult = await query.ReadPage<BalancoPatrimonialVw, BPViewDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa > 0)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (request.Filter.Ano > 0)
                        q = q.Where(e => e.Ano == request.Filter.Ano);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Codigo).ThenByDescending(e => e.Ano)
                        : q.OrderBy(e => e.Codigo).ThenBy(e => e.Ano);

                    return q;
                },
                mapFunc: data =>
                {
                    var dtos = _mapper.Map<List<BPViewDto>>(data);
                    foreach (var dto in dtos)
                    {
                        dto.ValorNormalizado = _normalizador.Normalizar(dto.Codigo, dto.ValCtaRefFin, dto.IndValCtaRefFin);
                        if (dto.ValCtaRefIni.HasValue)
                            dto.ValorInicialNormalizado = _normalizador.Normalizar(dto.Codigo, dto.ValCtaRefIni, dto.IndValCtaRefIni);
                    }
                    return dtos;
                }
            );

            return pagedResult;
        }
    }
}
