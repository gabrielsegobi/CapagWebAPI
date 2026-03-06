using Application.Exceptions.Empresas;
using Application.Queries.Indicadores;
using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Json;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Handlers.Indicadores
{
    public class GetAllCalcIndicaoresHandler : IRequestHandler<GetAllCalcIndicaoresQuery, PagedApiResponse<CalcIndicadoresDto>>
    {
        private readonly IBaseRepository<Indicador> _indicadorRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public GetAllCalcIndicaoresHandler(IBaseRepository<Indicador> indicadorRepository, IMapper mapper, IBaseRepository<Empresa> empresaRepository)
        {
            _indicadorRepository = indicadorRepository;
            _mapper = mapper;
            _empresaRepository = empresaRepository;
        }

        public async Task<PagedApiResponse<CalcIndicadoresDto>> Handle(GetAllCalcIndicaoresQuery request, CancellationToken cancellationToken)
        {

            var empresaExiste = await _empresaRepository
                 .Query()
                 .AnyAsync(e => e.IdEmpresa == request.IdEmpresa, cancellationToken);

            if (!empresaExiste)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            var query = _indicadorRepository
                .Query()
                .Where(i => i.IdEmpresa == request.IdEmpresa);

            if (request.Filter.Ano.HasValue)
            {
                query = query
                    .Where(i => i.ValoresAnuais
                        .Any(v => v.Ano == request.Filter.Ano.Value))
                    .Include(i => i.ValoresAnuais
                        .Where(v => v.Ano == request.Filter.Ano.Value));
            }
            else
            {
                query = query.Include(i => i.ValoresAnuais);
            }
            var pagedResult = await query.ReadPage<Indicador, CalcIndicadoresDto>(
                request.Filter,
                applyFilters: q =>
                {

                    if (request.Filter.IdIndicador.HasValue)
                        q = q.Where(e => e.IdIndicador == request.Filter.IdIndicador);


                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "nome" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Nome)
                            : q.OrderBy(e => e.Nome),

                        "saude_empresa" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.SaudeEmpresa)
                            : q.OrderBy(e => e.SaudeEmpresa),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.IdIndicador)
                            : q.OrderBy(e => e.IdIndicador)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<CalcIndicadoresDto>>(data)
            );

            var basePath = AppContext.BaseDirectory;
            var jsonPath = Path.Combine(basePath, "Domain", "Resources", "Indicadores.json");
            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonPath);
            var formulas = JsonSerializer.Deserialize<List<FormulaJson>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (formulas != null)
            {
                var formulasDict = formulas.ToDictionary(f => f.Nome, f => f.Desc_Formula);

                foreach (var indicador in pagedResult.Data)
                {
                    foreach (var valorAnual in indicador.ValoresAnuais)
                    {
                        if (formulasDict.TryGetValue(indicador.Nome, out var valorFormula))
                        {
                            valorAnual.Formula = valorFormula;
                            continue;
                        }
                        valorAnual.Formula = "Fórmula não encontrada";
                    }
                }
            }
            return pagedResult;
        }
    }
}
