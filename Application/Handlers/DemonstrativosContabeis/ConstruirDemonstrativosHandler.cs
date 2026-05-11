using Application.Commands.DemonstrativosContabeis;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class ConstruirDemonstrativosHandler : IRequestHandler<ConstruirDemonstrativosCommand, ConstruirDemonstrativosResponse>
    {
        private readonly IBaseRepository<RegDefi> _defisRepository;
        private readonly IBaseRepository<RegPgdasd> _pgdasdRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly Infrastructure.Context.CPGDbContext _db;

        public ConstruirDemonstrativosHandler(
            IBaseRepository<RegDefi> defisRepository,
            IBaseRepository<RegPgdasd> pgdasdRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            ICurrentUserService currentUser,
            Infrastructure.Context.CPGDbContext db)
        {
            _defisRepository = defisRepository;
            _pgdasdRepository = pgdasdRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _currentUser = currentUser;
            _db = db;
        }

        public async Task<ConstruirDemonstrativosResponse> Handle(ConstruirDemonstrativosCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

            if (request.IdEmpresa <= 0)
                throw new ArgumentException("id_empresa inválido.");

            if (request.Anos == null || request.Anos.Count == 0)
                throw new ArgumentException("Lista de anos inválida.");

            var tenantHeader = _currentUser.TenantId;
            if (!tenantHeader.HasValue)
                throw new ArgumentException("Tenant não informado.");

            if (request.IdTenant != tenantHeader.Value)
                throw new ArgumentException("id_tenant não confere com o tenant do header.");

            var idTenant = tenantHeader.Value;

            var anos = request.Anos
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            var response = new ConstruirDemonstrativosResponse
            {
                Sucesso = true
            };

            foreach (var ano in anos)
            {
                var resultadoAno = new ConstruirDemonstrativosAnoResultado();
                response.Resultados[ano] = resultadoAno;

                await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    if (request.Sobrescrever)
                    {
                        var now = DateTimeHelper.GetDateTimeNow();
                        var existentes = await _demonstrativoRepository
                            .Query(dc => dc.IdTenant == idTenant &&
                                         dc.IdEmpresa == request.IdEmpresa &&
                                         dc.Ano == ano &&
                                         dc.DeletedAt == null,
                                 asNoTracking: false)
                            .ToListAsync(cancellationToken);

                        foreach (var item in existentes)
                        {
                            item.DeletedAt = now;
                            item.UpdatedAt = now;
                        }

                        if (existentes.Count > 0)
                        {
                            _demonstrativoRepository.UpdateRange(existentes);
                            await _demonstrativoRepository.SaveChangesAsync();
                        }

                        resultadoAno.Deletados = existentes.Count;
                    }

                    var defisRows = await _defisRepository
                        .Query(d => d.IdEmpresa == request.IdEmpresa &&
                                    d.Periodo.Year == ano)
                        .ToListAsync(cancellationToken);

                    var pgdasdRows = await _pgdasdRepository
                        .Query(p => p.IdEmpresa == request.IdEmpresa)
                        .ToListAsync(cancellationToken);

                    var pgdasdAno = pgdasdRows
                        .Select(p => new { Row = p, Dt = TryParsePeriodoPgdasd(p.Periodo) })
                        .Where(x => x.Dt.HasValue && x.Dt.Value.Year == ano)
                        .ToList();

                    var nowInsert = DateTimeHelper.GetDateTimeNow();
                    var itensParaInserir = new List<DemonstrativoContabil>();

                    // Alertas de negócio (processamento continua)
                    var alertasAno = resultadoAno.Alertas;

                    // Despesas zeradas (DEFIS)
                    var despesasDefis = GetValorDefis(defisRows, "Total de despesas no período");
                    if (despesasDefis == 0)
                    {
                        alertasAno.Add(
                            "DESPESAS_ZERADAS: O campo 'Total de despesas no período' da DEFIS está zerado. " +
                            "Verifique se houve omissão na declaração."
                        );
                    }

                    // Divergência receita DEFIS x PGDASD
                    var receitaDefis = GetValorDefis(defisRows, "Total de entradas no período");
                    var receitaPgdasd = pgdasdAno.Sum(x => x.Row.ReceitaBruta);
                    var delta = Math.Abs(receitaDefis - receitaPgdasd);
                    if (receitaDefis > 0 && receitaPgdasd > 0 && delta > 1m)
                    {
                        alertasAno.Add(
                            $"DIVERGENCIA_RECEITA: Receita no DEFIS (R$ {receitaDefis:N2}) difere do " +
                            $"somatório do PGDASD (R$ {receitaPgdasd:N2}). Diferença: R$ {delta:N2}. " +
                            "Verifique a consistência das declarações."
                        );
                    }

                    // DEFIS -> anual (7 itens)
                    itensParaInserir.AddRange(BuildDefisAnual(idTenant, request.IdEmpresa, ano, defisRows, nowInsert, alertasAno));

                    // PGDASD -> mensal + anual
                    itensParaInserir.AddRange(BuildPgdasd(idTenant, request.IdEmpresa, ano, pgdasdAno.Select(x => x.Row), nowInsert));

                    await _demonstrativoRepository.AddRangeAsync(itensParaInserir);
                    await _demonstrativoRepository.SaveChangesAsync();

                    await tx.CommitAsync(cancellationToken);

                    resultadoAno.Inseridos = itensParaInserir.Count;
                    response.TotalInseridos += itensParaInserir.Count;
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync(cancellationToken);
                    resultadoAno.Erros.Add(ex.Message);
                    response.Sucesso = false;
                    continue;
                }
            }

            return response;
        }

        private static IEnumerable<DemonstrativoContabil> BuildDefisAnual(
            long idTenant,
            long idEmpresa,
            int ano,
            IEnumerable<RegDefi> defisRows,
            DateTime now,
            List<string> alertas)
        {
            decimal estoqueInicial = GetValorDefis(defisRows, "Estoque inicial");
            decimal estoqueFinal = GetValorDefis(defisRows, "Estoque final");
            decimal caixaInicio = GetValorDefis(defisRows, "Saldo em caixa/banco no início");
            decimal caixaFinal = GetValorDefis(defisRows, "Saldo em caixa/banco no final");
            decimal compras = GetValorDefis(defisRows, "Total de aquisições de mercadorias");
            decimal devVendas = GetValorDefis(defisRows, "Total de devoluções de vendas");
            decimal devCompras = GetValorDefis(defisRows, "Total de devoluções de compras");
            decimal despesas = GetValorDefis(defisRows, "Total de despesas no período");

            decimal cmv = estoqueInicial + compras - devCompras - estoqueFinal;
            if (cmv < 0)
            {
                alertas.Add(
                    $"CMV_NEGATIVO: O CMV calculado para {ano} resultou em R$ {cmv:N2}. " +
                    $"Fórmula: Est.Ini ({estoqueInicial:N2}) + Compras ({compras:N2}) " +
                    $"- Dev.Compras ({devCompras:N2}) - Est.Final ({estoqueFinal:N2}). " +
                    "Registrado como R$ 0,00. Verifique se o estoque inicial foi declarado corretamente."
                );
                cmv = 0;
            }

            var dtIni = new DateOnly(ano, 1, 1);
            var dtFin = new DateOnly(ano, 12, 31);

            DemonstrativoContabil NewBase(
                string codigo,
                string descricao,
                char tipo,
                byte nivel)
                => new DemonstrativoContabil
                {
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = "ANUAL",
                    Ano = ano,
                    Codigo = codigo,
                    Descricao = descricao,
                    Tipo = tipo,
                    Nivel = nivel,
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                };

            var lista = new List<DemonstrativoContabil>
            {
                new()
                {
                    // 1
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = "ANUAL",
                    Ano = ano,
                    Codigo = "1.01.01",
                    Descricao = "DISPONIBILIDADES/CAIXAS E EQUIVALENTES",
                    Tipo = 'A',
                    Nivel = 3,
                    ValCtaRefIni = caixaInicio,
                    IndValCtaRefIni = 'D',
                    ValCtaRefDeb = null,
                    ValCtaRefCred = null,
                    ValCtaRefFin = caixaFinal,
                    IndValCtaRefFin = 'D',
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                },
                new()
                {
                    // 2
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = "ANUAL",
                    Ano = ano,
                    Codigo = "1.01.03",
                    Descricao = "ESTOQUES",
                    Tipo = 'A',
                    Nivel = 3,
                    ValCtaRefIni = estoqueInicial,
                    IndValCtaRefIni = 'D',
                    ValCtaRefDeb = null,
                    ValCtaRefCred = null,
                    ValCtaRefFin = estoqueFinal,
                    IndValCtaRefFin = 'D',
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                },
                CreateResultadoDeb(NewBase("3.01.01.01.02", "DEDUÇÕES DA RECEITA BRUTA - DEV. VENDAS", 'R', 5), devVendas),
                CreateResultadoDebFin(NewBase("3.01.01.03.01", "COMPRAS DE MERCADORIAS", 'R', 5), compras, 'D'),
                CreateResultadoDebFin(NewBase("3.01.01.03", "CUSTO DOS BENS E SERVIÇOS (CMV)", 'R', 4), cmv, 'D'),
                CreateResultadoCred(NewBase("3.01.01.03.02", "DEVOLUÇÕES DE COMPRAS", 'R', 5), devCompras),
                CreateResultadoDebFin(NewBase("3.01.01.07.01", "DESPESAS OPERACIONAIS", 'R', 4), despesas, 'D')
            };

            return lista;
        }

        private static IEnumerable<DemonstrativoContabil> BuildPgdasd(
            long idTenant,
            long idEmpresa,
            int ano,
            IEnumerable<RegPgdasd> pgdasdRowsAno,
            DateTime now)
        {
            var parsed = pgdasdRowsAno
                .Select(r => new { Row = r, Dt = TryParsePeriodoPgdasd(r.Periodo) })
                .Where(x => x.Dt.HasValue)
                .Select(x => new { x.Row, Dt = x.Dt!.Value })
                .ToList();

            var mensal = parsed
                .GroupBy(x => new { x.Dt.Year, x.Dt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Receita = g.Sum(x => x.Row.ReceitaBruta),
                    Das = g.Sum(x => x.Row.TotalDebito)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList();

            var lista = new List<DemonstrativoContabil>();

            foreach (var m in mensal)
            {
                var dtIni = new DateOnly(m.Year, m.Month, 1);
                var dtFin = new DateOnly(m.Year, m.Month, DateTime.DaysInMonth(m.Year, m.Month));
                var perApur = $"{m.Year:D4}-{m.Month:D2}";

                lista.Add(new DemonstrativoContabil
                {
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = perApur,
                    Ano = m.Year,
                    Codigo = "3.01.01.01",
                    Descricao = "RECEITA BRUTA",
                    Tipo = 'R',
                    Nivel = 4,
                    ValCtaRefIni = null,
                    IndValCtaRefIni = null,
                    ValCtaRefDeb = null,
                    ValCtaRefCred = m.Receita,
                    ValCtaRefFin = m.Receita,
                    IndValCtaRefFin = 'C',
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                });

                // ── DAS mensal — DRE (imposto sobre vendas) ───────────────────────────────
                lista.Add(new DemonstrativoContabil
                {
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = perApur,
                    Ano = m.Year,
                    Codigo = "3.01.01.01.02.09",
                    Descricao = "IMPOSTOS SOBRE VENDAS - DAS (SIMPLES NACIONAL)",
                    Tipo = 'R',
                    Nivel = 6,
                    ValCtaRefIni = null,
                    IndValCtaRefIni = null,
                    ValCtaRefDeb = m.Das,
                    ValCtaRefCred = null,
                    ValCtaRefFin = m.Das,
                    IndValCtaRefFin = 'D',
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                });

                // ── DAS mensal — Passivo (tributo a recolher) ─────────────────────────────
                lista.Add(new DemonstrativoContabil
                {
                    IdTenant = idTenant,
                    IdEmpresa = idEmpresa,
                    DtIni = dtIni,
                    DtIniApur = dtIni,
                    DtFinApur = dtFin,
                    PerApur = perApur,
                    Ano = m.Year,
                    Codigo = "2.01.01.09.28",
                    Descricao = "OUTROS TRIBUTOS A RECOLHER - CIRCULANTE (DAS)",
                    Tipo = 'P',
                    Nivel = 5,
                    ValCtaRefIni = null,
                    IndValCtaRefIni = null,
                    ValCtaRefDeb = m.Das,
                    ValCtaRefCred = null,
                    ValCtaRefFin = m.Das,
                    IndValCtaRefFin = 'D',
                    TipoTrib = "SIMPLES_NACIONAL",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                });
            }

            var totalReceita = mensal.Sum(x => x.Receita);
            var totalDas = mensal.Sum(x => x.Das);

            var dtIniAno = new DateOnly(ano, 1, 1);
            var dtFinAno = new DateOnly(ano, 12, 31);

            lista.Add(new DemonstrativoContabil
            {
                IdTenant = idTenant,
                IdEmpresa = idEmpresa,
                DtIni = dtIniAno,
                DtIniApur = dtIniAno,
                DtFinApur = dtFinAno,
                PerApur = "ANUAL",
                Ano = ano,
                Codigo = "3.01.01.01",
                Descricao = "RECEITA BRUTA",
                Tipo = 'R',
                Nivel = 4,
                ValCtaRefIni = null,
                IndValCtaRefIni = null,
                ValCtaRefDeb = null,
                ValCtaRefCred = totalReceita,
                ValCtaRefFin = totalReceita,
                IndValCtaRefFin = 'C',
                TipoTrib = "SIMPLES_NACIONAL",
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            });

            // ── DAS anual — DRE ───────────────────────────────────────────────────────
            lista.Add(new DemonstrativoContabil
            {
                IdTenant = idTenant,
                IdEmpresa = idEmpresa,
                DtIni = dtIniAno,
                DtIniApur = dtIniAno,
                DtFinApur = dtFinAno,
                PerApur = "ANUAL",
                Ano = ano,
                Codigo = "3.01.01.01.02.09",
                Descricao = "IMPOSTOS SOBRE VENDAS - DAS (SIMPLES NACIONAL)",
                Tipo = 'R',
                Nivel = 6,
                ValCtaRefIni = null,
                IndValCtaRefIni = null,
                ValCtaRefDeb = totalDas,
                ValCtaRefCred = null,
                ValCtaRefFin = totalDas,
                IndValCtaRefFin = 'D',
                TipoTrib = "SIMPLES_NACIONAL",
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            });

            // ── DAS anual — Passivo ───────────────────────────────────────────────────
            lista.Add(new DemonstrativoContabil
            {
                IdTenant = idTenant,
                IdEmpresa = idEmpresa,
                DtIni = dtIniAno,
                DtIniApur = dtIniAno,
                DtFinApur = dtFinAno,
                PerApur = "ANUAL",
                Ano = ano,
                Codigo = "2.01.01.09.28",
                Descricao = "OUTROS TRIBUTOS A RECOLHER - CIRCULANTE (DAS)",
                Tipo = 'P',
                Nivel = 5,
                ValCtaRefIni = null,
                IndValCtaRefIni = null,
                ValCtaRefDeb = totalDas,
                ValCtaRefCred = null,
                ValCtaRefFin = totalDas,
                IndValCtaRefFin = 'D',
                TipoTrib = "SIMPLES_NACIONAL",
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            });

            return lista;
        }

        private static DemonstrativoContabil CreateResultadoDeb(DemonstrativoContabil dc, decimal valorDeb)
        {
            dc.ValCtaRefDeb = valorDeb;
            return dc;
        }

        private static DemonstrativoContabil CreateResultadoCred(DemonstrativoContabil dc, decimal valorCred)
        {
            dc.ValCtaRefCred = valorCred;
            return dc;
        }

        private static DemonstrativoContabil CreateResultadoDebFin(DemonstrativoContabil dc, decimal valorDeb, char indFin)
        {
            dc.ValCtaRefDeb = valorDeb;
            dc.ValCtaRefFin = valorDeb;
            dc.IndValCtaRefFin = indFin;
            return dc;
        }

        private static decimal GetValorDefis(IEnumerable<RegDefi> rows, string chave)
        {
            return rows
                .Where(r => r.Descricao != null &&
                            r.Descricao.Contains(chave, StringComparison.OrdinalIgnoreCase))
                .Sum(r => r.Valor);
        }

        private static DateTime? TryParsePeriodoPgdasd(string? periodo)
        {
            if (string.IsNullOrWhiteSpace(periodo))
                return null;

            var s = periodo.Trim();
            if (s.Length == 7)
                s += "-01";

            if (DateTime.TryParse(s, out var dt))
                return dt;

            return null;
        }
    }
}

