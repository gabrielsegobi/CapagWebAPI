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
                x.DeletedAt == null &&
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

                // Fechamento do ano anterior para [I]: fora do AnosFiltro (senão o ano N-1
                // fica invisível quando a janela começa em N). Aceita T04 ou A00.
                var fechamentoAnteriorLookup = new Dictionary<(string Codigo, int Ano), DemonstrativoContabil>();
                if (codigosComTrimestral.Count > 0 && anosAnterioresNecessarios.Count > 0)
                {
                    var fechamentosAnoAnterior = await _baseRepository
                        .Query(x =>
                            x.IdEmpresa == request.IdEmpresa &&
                            x.DeletedAt == null &&
                            !string.IsNullOrWhiteSpace(x.Codigo) &&
                            codigosComTrimestral.Contains(x.Codigo) &&
                            anosAnterioresNecessarios.Contains(x.Ano) &&
                            (x.PerApur == "T04" || x.PerApur == "A00"))
                        .ToListAsync(cancellationToken);

                    fechamentoAnteriorLookup = fechamentosAnoAnterior
                        .GroupBy(x => (x.Codigo, x.Ano))
                        .ToDictionary(
                            g => g.Key,
                            g => g.FirstOrDefault(x => x.PerApur == "T04")
                                 ?? g.First(x => x.PerApur == "A00"));
                }

                var reagrupado = agrupado.Select(g =>
                {
                    fechamentoAnteriorLookup.TryGetValue((g.Codigo, g.Ano - 1), out var fechamentoAnterior);

                    var a00 = g.Items.FirstOrDefault(x => x.PerApur == "A00");
                    var t04 = g.Items.FirstOrDefault(x => x.PerApur == "T04");

                    var isDre = g.Items.All(x => x.ValCtaRefIni == null);

                    // Fórmulas de indicadores usam apenas magnitudes positivas: o indicador
                    // ECD (D/C) nunca entra no cálculo.
                    // [I] = fechamento do exercício anterior (T04 ou A00).
                    // Se não houver N-1, [I] permanece ausente e a fórmula trata como 0.
                    decimal? valorIni = null;
                    if (!isDre)
                    {
                        if (g.IsTrimestral)
                        {
                            if (fechamentoAnterior != null)
                                valorIni = SaldoContabilHelper.Magnitude(fechamentoAnterior);
                        }
                        else if (a00 != null)
                        {
                            valorIni = SaldoContabilHelper.Magnitude(a00, usarInicial: true);
                        }
                    }

                    decimal valorFin;
                    if (isDre)
                    {
                        // DRE trimestral: soma das magnitudes (val_cta_ref_fin), alinhada à dre-analise.
                        // DRE anual (A00): também magnitude — indicador D/C ignorado nas fórmulas.
                        valorFin = g.IsTrimestral
                            ? g.Items.Sum(x => SaldoContabilHelper.Magnitude(x.ValCtaRefFin))
                            : SaldoContabilHelper.Magnitude(a00);
                    }
                    else
                    {
                        var fonteFechamento = g.IsTrimestral ? t04 : a00;
                        valorFin = SaldoContabilHelper.Magnitude(fonteFechamento);
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

                // Síntese do exercício anterior sem lançamentos próprios: o saldo de abertura
                // (val_cta_ref_ini) do primeiro exercício com dados equivale ao fechamento do
                // exercício anterior. Permite que indicadores de balanço tenham valor no ano mais
                // antigo da janela mesmo sem ECD próprio. DRE (fluxo) não é sintetizada, pois a
                // abertura patrimonial não representa resultado do período.
                if (anosFiltroAtivo && agrupado.Count > 0)
                {
                    var anoMin = agrupado.Min(x => x.Ano);
                    var anoSintetico = anoMin - 1;

                    if (request.AnosFiltro!.Contains(anoSintetico) &&
                        agrupado.All(x => x.Ano != anoSintetico))
                    {
                        foreach (var g in agrupado.Where(x => x.Ano == anoMin))
                        {
                            var isDre = g.Items.All(x => x.ValCtaRefIni == null);
                            if (isDre)
                                continue;

                            var abertura = ObterAberturaExercicio(g.Items, g.IsTrimestral);
                            if (!abertura.HasValue)
                                continue;

                            resultado.Add(new DClByAnoAndCodigoDto
                            {
                                Codigo = g.Codigo,
                                Ano = anoSintetico,
                                Valor = abertura.Value
                            });
                        }
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

        /// <summary>
        /// Magnitude de abertura do exercício: T01 no regime trimestral, A00 no anual.
        /// Corresponde ao fechamento do exercício imediatamente anterior (sem sinal D/C).
        /// </summary>
        private static decimal? ObterAberturaExercicio(List<DemonstrativoContabil> items, bool isTrimestral)
        {
            if (isTrimestral)
            {
                var abertura = items
                    .Where(x => x.PerApur != null && x.PerApur.StartsWith("T0"))
                    .OrderBy(x => x.PerApur)
                    .FirstOrDefault();

                return abertura != null
                    ? SaldoContabilHelper.Magnitude(abertura, usarInicial: true)
                    : null;
            }

            var a00 = items.FirstOrDefault(x => x.PerApur == "A00");
            return a00 != null
                ? SaldoContabilHelper.Magnitude(a00, usarInicial: true)
                : null;
        }
    }
}
