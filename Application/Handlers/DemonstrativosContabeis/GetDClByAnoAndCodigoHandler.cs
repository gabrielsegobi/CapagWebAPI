using Application.Helpers;
using Application.Queries.DemonstrativosContabeis;
using AutoMapper;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var anosFiltroAtivo = request.AnosFiltro != null && request.AnosFiltro.Any();

            var query = _baseRepository.Query(x =>
                x.IdEmpresa == request.IdEmpresa &&
                !string.IsNullOrWhiteSpace(x.Codigo) &&
                (!anosFiltroAtivo || request.AnosFiltro!.Contains(x.Ano)));

            var resultado = new List<DClByAnoAndCodigoDto>();

            if (request.Ano && request.SomarPeriodosNoAno)
            {
                var agrupado = await query
                    .GroupBy(x => new { x.Codigo, x.Ano })
                    .Select(g => new
                    {
                        g.Key.Codigo,
                        g.Key.Ano,
                        TotalValCtaRefFin = g.Sum(x => x.ValCtaRefFin),
                        TotalValCtaRefIni = g.Sum(x => x.ValCtaRefIni)
                    })
                    .ToListAsync(cancellationToken);

                foreach (var item in agrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Ano = item.Ano,
                        Valor = item.TotalValCtaRefFin ?? 0
                    });

                    if (item.TotalValCtaRefIni != null)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Ano = item.Ano,
                            Valor = item.TotalValCtaRefIni ?? 0
                        });
                    }
                }

                return resultado;
            }

            if (request.Ano)
            {
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

                var t04AnoAnterior = await query
                    .Where(x => x.PerApur == "T04"
                            && codigosComTrimestral.Contains(x.Codigo)
                            && anosAnterioresNecessarios.Contains(x.Ano))
                    .ToListAsync(cancellationToken);

                var t04Lookup = t04AnoAnterior
                    .ToDictionary(x => (x.Codigo, x.Ano));

                var reagrupado = agrupado.Select(g =>
                {
                    t04Lookup.TryGetValue((g.Codigo, g.Ano - 1), out var t04Ant);

                    var a00 = g.Items.FirstOrDefault(x => x.PerApur == "A00");
                    var t04 = g.Items.FirstOrDefault(x => x.PerApur == "T04");

                    var isDre = g.Items.All(x => x.ValCtaRefIni == null);

                    decimal? valorIni = null;
                    if (!isDre)
                    {
                        if (g.IsTrimestral && t04Ant != null)
                            valorIni = SaldoContabilHelper.SaldoAssinado(t04Ant);
                        else if (!g.IsTrimestral && a00 != null)
                            valorIni = SaldoContabilHelper.SaldoAssinado(a00, usarInicial: true);
                    }

                    decimal valorFin;
                    if (isDre)
                    {
                        valorFin = g.IsTrimestral
                            ? g.Items.Sum(x => SaldoContabilHelper.SaldoAssinado(x.ValCtaRefFin, x.IndValCtaRefFin))
                            : SaldoContabilHelper.SaldoAssinado(a00);
                    }
                    else
                    {
                        var fonteFechamento = g.IsTrimestral ? t04 : a00;
                        valorFin = SaldoContabilHelper.SaldoAssinado(fonteFechamento);
                    }

                    return new
                    {
                        g.Codigo,
                        g.Ano,
                        ValorIni = valorIni,
                        ValorFin = valorFin
                    };
                }).ToList();

                foreach (var item in reagrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Ano = item.Ano,
                        Valor = item.ValorFin
                    });

                    if (item.ValorIni.HasValue)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Ano = item.Ano,
                            Valor = item.ValorIni.Value
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
