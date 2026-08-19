using Application.Helpers;
using Application.Interfaces;
using Application.Queries.DemonstrativosContabeis;
using Application.Services.Demonstrativos;
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
        private readonly INormalizadorSinalService _normalizador;

        public GetDClByAnoAndCodigoHandler(
            IBaseRepository<DemonstrativoContabil> baseRepository,
            IMapper mapper,
            INormalizadorSinalService normalizador)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _normalizador = normalizador;
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

                var anosAnterioresNecessarios = agrupado
                    .Select(x => x.Ano - 1)
                    .Distinct()
                    .ToList();
                var codigos = agrupado
                    .Select(x => x.Codigo)
                    .Distinct()
                    .ToList();

                // [I] = fechamento do ano anterior (T04 ou A00), fora do AnosFiltro.
                var fechamentoAnteriorPorConta = new Dictionary<(string Codigo, int Ano), List<DemonstrativoContabil>>();
                if (codigos.Count > 0 && anosAnterioresNecessarios.Count > 0)
                {
                    var fechamentosAnoAnterior = await _baseRepository
                        .Query(x =>
                            x.IdEmpresa == request.IdEmpresa &&
                            x.DeletedAt == null &&
                            !string.IsNullOrWhiteSpace(x.Codigo) &&
                            codigos.Contains(x.Codigo) &&
                            anosAnterioresNecessarios.Contains(x.Ano) &&
                            (x.PerApur == "T04" || x.PerApur == "A00"))
                        .ToListAsync(cancellationToken);

                    fechamentoAnteriorPorConta = fechamentosAnoAnterior
                        .GroupBy(x => (x.Codigo, x.Ano))
                        .ToDictionary(g => g.Key, g => g.ToList());
                }

                var reagrupado = agrupado.Select(g =>
                {
                    var isDre = g.Items.All(x => x.ValCtaRefIni == null);

                    // [I] = saldo final do ano anterior (T04 trimestral, A00 anual). Sem o ano N-1 → 0.
                    decimal? valorIni = null;
                    if (!isDre)
                    {
                        fechamentoAnteriorPorConta.TryGetValue((g.Codigo, g.Ano - 1), out var anteriores);
                        var fechamentoAnterior = anteriores != null
                            ? DemonstrativoPeriodoHelper.FechamentoParaSaldoInicial(anteriores, x => x.PerApur)
                            : null;
                        valorIni = fechamentoAnterior != null
                            ? NormalizarRegistro(fechamentoAnterior, g.Codigo)
                            : 0m;
                    }

                    decimal valorFin;
                    if (isDre && g.IsTrimestral)
                    {
                        valorFin = DemonstrativoLeituraService.ConsolidarDreTrimestral(
                            g.Codigo, g.Items, _normalizador).Normalizado;
                    }
                    else
                    {
                        var fonteFechamento = DemonstrativoPeriodoHelper.FechamentoDoExercicio(g.Items, x => x.PerApur);
                        valorFin = NormalizarRegistro(fonteFechamento, g.Codigo);
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

                            var abertura = ObterAberturaExercicio(g.Items, g.IsTrimestral, g.Codigo);
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
        /// Abertura do exercício: T01 no regime trimestral, A00 no anual.
        /// Corresponde ao fechamento do exercício imediatamente anterior (política ValorParaFormula).
        /// </summary>
        private decimal? ObterAberturaExercicio(List<DemonstrativoContabil> items, bool isTrimestral, string codigo)
        {
            if (isTrimestral)
            {
                var abertura = items
                    .Where(x => x.PerApur != null && x.PerApur.StartsWith("T0"))
                    .OrderBy(x => x.PerApur)
                    .FirstOrDefault();

                return abertura != null
                    ? NormalizarRegistro(abertura, codigo, usarInicial: true)
                    : null;
            }

            var a00 = items.FirstOrDefault(x => x.PerApur == "A00");
            return a00 != null
                ? NormalizarRegistro(a00, codigo, usarInicial: true)
                : null;
        }

        private decimal NormalizarRegistro(DemonstrativoContabil? registro, string codigo, bool usarInicial = false)
        {
            if (registro == null)
                return 0m;

            return usarInicial
                ? _normalizador.Normalizar(codigo, registro.ValCtaRefIni, registro.IndValCtaRefIni)
                : _normalizador.Normalizar(codigo, registro.ValCtaRefFin, registro.IndValCtaRefFin);
        }
    }
}
