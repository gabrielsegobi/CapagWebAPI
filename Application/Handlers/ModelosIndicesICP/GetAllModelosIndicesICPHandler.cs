using Application.Queries.ModelosIndicesICP;
using AutoMapper;
using Domain.Contracts.ModelosIndicesICP;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.ModelosIndicesICP
{
    public class GetAllModelosIndicesICPHandler : IRequestHandler<GetAllModelosIndicesICPQuery, PagedApiResponse<ModeloIndiceICPDto>>
    {
        private readonly IBaseRepository<ModeloIndiceICP> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllModelosIndicesICPHandler(IBaseRepository<ModeloIndiceICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<ModeloIndiceICPDto>> Handle(GetAllModelosIndicesICPQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query().Include(i => i.Resultados);

            var pagedResult = await query.ReadPage<ModeloIndiceICP, ModeloIndiceICPDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Nome))
                        q = q.Where(e => e.Nome.Contains(request.Filter.Nome.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.Formula))
                        q = q.Where(e => e.Formula.Contains(request.Filter.Formula.Trim()));

                    if (request.Filter.IdEmpresa.HasValue)
                        q = q.Where(e => e.Resultados.Any(r => r.IdEmpresa == request.Filter.IdEmpresa.Value));

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Nome)
                        : q.OrderBy(e => e.Nome);

                    return q;
                },
                mapFunc: data =>
                {
                    var mapped = _mapper.Map<IEnumerable<ModeloIndiceICPDto>>(data);

                    if (request.Filter.IdEmpresa.HasValue)
                    {
                        foreach (var m in mapped)
                            m.Resultados = m.Resultados
                                .Where(r => r.IdEmpresa == request.Filter.IdEmpresa.Value)
                                .ToList();
                    }

                    return mapped;
                }
            );

            return pagedResult;
        }
    }
}
