using Application.Queries.CapagCalculadoraResultados;
using AutoMapper;
using Domain.Contracts.CapagCalculadoraResultados;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CapagCalculadoraResultados
{
    public class GetAllCapagCalculadoraResultadosHandler : IRequestHandler<GetAllCapagCalculadoraResultadosQuery, PagedApiResponse<CapagCalculadoraResultadoDto>>
    {
        private readonly IBaseRepository<CapagCalculadoraResultado> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllCapagCalculadoraResultadosHandler(IBaseRepository<CapagCalculadoraResultado> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<CapagCalculadoraResultadoDto>> Handle(GetAllCapagCalculadoraResultadosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<CapagCalculadoraResultado, CapagCalculadoraResultadoDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa.HasValue)
                        q = q.Where(e => e.IdEmpresa == request.Filter.IdEmpresa);

                    if (!string.IsNullOrWhiteSpace(request.Filter.Modelo))
                        q = q.Where(e => e.Modelo == request.Filter.Modelo.Trim());

                    if (!string.IsNullOrWhiteSpace(request.Filter.Classificacao))
                        q = q.Where(e => e.Classificacao == request.Filter.Classificacao.Trim().ToUpperInvariant());

                    if (request.Filter.Parcial.HasValue)
                        q = q.Where(e => e.Parcial == request.Filter.Parcial.Value);

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "idempresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdEmpresa)
                            : q.OrderBy(e => e.IdEmpresa),

                        "modelo" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Modelo)
                            : q.OrderBy(e => e.Modelo),

                        "classificacao" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Classificacao)
                            : q.OrderBy(e => e.Classificacao),

                        "valorcapag" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.ValorCapag)
                            : q.OrderBy(e => e.ValorCapag),

                        "indice" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Indice)
                            : q.OrderBy(e => e.Indice),

                        "parcial" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Parcial)
                            : q.OrderBy(e => e.Parcial),

                        "dateupdate" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.DateUpdate)
                            : q.OrderBy(e => e.DateUpdate),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Id)
                            : q.OrderBy(e => e.Id)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<CapagCalculadoraResultadoDto>>(data)
            );

            return pagedResult;
        }
    }
}
