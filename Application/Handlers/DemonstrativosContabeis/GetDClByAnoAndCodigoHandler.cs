using Application.Queries.DemonstrativosContabeis;
using AutoMapper;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class GetDClByAnoAndCodigoHandler : IRequestHandler<GetDClByAnoAndCodigoQuery, List<DClByAnoAndCodigoDto>>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _baseRepository;
        private readonly IMapper _mapper;

        public GetDClByAnoAndCodigoHandler(IBaseRepository<DemonstrativoContabil> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<DClByAnoAndCodigoDto>> Handle(GetDClByAnoAndCodigoQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query(x =>
                x.IdEmpresa == request.IdEmpresa &&
                !string.IsNullOrWhiteSpace(x.Codigo));

            var resultado = new List<DClByAnoAndCodigoDto>();

            if (request.Ano)
            {
                // 1. Primeira query normal (sem cross-ano)
                var agrupado = await query
                    .GroupBy(x => new { x.Codigo, x.Ano })
                    .Select(g => new
                    {
                        g.Key.Codigo,
                        g.Key.Ano,
                        IsTrimestral = g.Any(x => x.PerApur.StartsWith("T0")),
                        Items = g.ToList()
                    })
                    .ToListAsync(cancellationToken);

                // 2. Extrair os pares Codigo+Ano que precisam do T04 anterior
                var codigosComTrimestral = agrupado
                    .Where(x => x.IsTrimestral)
                    .Select(x => x.Codigo)
                    .Distinct()
                    .ToList();

                var anosAnterioresNecessarios = agrupado
                    .Where(x => x.IsTrimestral)
                    .Select(x => x.Ano - 1)
                    .Distinct()
                    .ToList();

                // 3. EF Core consegue traduzir Contains para IN no SQL
                var t04AnoAnterior = await query
                    .Where(x => x.PerApur == "T04"
                            && codigosComTrimestral.Contains(x.Codigo)
                            && anosAnterioresNecessarios.Contains(x.Ano))
                    .ToListAsync(cancellationToken);

                // 4. Cruzamento em memória com lookup para performance
                var t04Lookup = t04AnoAnterior
                    .ToDictionary(x => (x.Codigo, x.Ano)); // (Codigo, AnoAnterior) -> registro

                // 5. Projeção final
                var reagrupado = agrupado.Select(g =>
                {
                    t04Lookup.TryGetValue((g.Codigo, g.Ano - 1), out var t04Ant);

                    var a00   = g.Items.FirstOrDefault(x => x.PerApur == "A00");
                    var t04   = g.Items.FirstOrDefault(x => x.PerApur == "T04");

                    var isDre = g.Items.All(x => x.ValCtaRefIni == null);

                    return new
                    {
                        g.Codigo,
                        g.Ano,
                        TotalValCtaRefIni = (!isDre && g.IsTrimestral)  ? t04Ant?.ValCtaRefFin : (!g.IsTrimestral) ? a00?.ValCtaRefIni : (decimal?)null,
                        TotalValCtaRefFin = (g.IsTrimestral) ? t04?.ValCtaRefFin : a00?.ValCtaRefFin,
                        TotalValCtaRefIniSum = (decimal?)null,
                        TotalValCtaRefFinSum = (g.IsTrimestral) ? g.Items.Sum(x => x.ValCtaRefFin) : a00?.ValCtaRefFin 
                    };                    
                }).ToList();

                foreach (var item in reagrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Ano = item.Ano,
                        Valor = item.TotalValCtaRefIni != null ? (item.TotalValCtaRefFin ?? 0) : (item.TotalValCtaRefFinSum ?? 0)
                    });

                    if (item.TotalValCtaRefIni != null)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Ano = item.Ano,
                            Valor = item.TotalValCtaRefIni != null ? (item.TotalValCtaRefIni ?? 0) :  (item.TotalValCtaRefIniSum ?? 0)
                        });
                    }
                }
            }
            else
            {
                var agrupado = await query
                    .GroupBy(x => x.Codigo)
                    .Select(g => new
                    {
                        Codigo = g.Key,
                        TotalValCtaRefFin = g.Sum(x => x.ValCtaRefFin),
                        TotalValCtaRefIni = g.Sum(x => x.ValCtaRefIni)
                    })
                    .ToListAsync(cancellationToken);

                foreach (var item in agrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Valor = item.TotalValCtaRefFin ?? 0
                    });

                    if (item.TotalValCtaRefIni != null)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Valor = item.TotalValCtaRefIni ?? 0
                        });
                    }
                }
            }

            return resultado;
        }

    }
}
