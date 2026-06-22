using Application.Commands.DemonstrativosContabeis;

using Application.Commands.Indicadores;

using Application.Commands.ResultadosIndicesICP;

using Application.Helpers;

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

        /// <summary>Período de apuração: sempre A00 (exercício anual ECD/SPED).</summary>

        private const string PerApurAnualEcd = "A00";



        /// <summary>

        /// Regime tributário exibido na UI (Lucro Real, Lucro Presumido, Simples Nacional).

        /// Este fluxo constrói demonstrativos no Simples Nacional.

        /// </summary>

        private const string TipoTribSimplesNacional = "Simples Nacional";



        private const string CodigoPatrimonioLiquido = "2.03";



        /// <summary>Nível contábil = quantidade de segmentos do código (ex.: 3.01.01.03.01 → 5).</summary>

        private static byte NivelDoCodigo(string codigo) =>

            (byte)codigo.Split('.', StringSplitOptions.RemoveEmptyEntries).Length;



        private readonly IBaseRepository<RegDefi> _defisRepository;

        private readonly IBaseRepository<RegPgdasd> _pgdasdRepository;

        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;

        private readonly ICurrentUserService _currentUser;

        private readonly Infrastructure.Context.CPGDbContext _db;

        private readonly IMediator _mediator;



        public ConstruirDemonstrativosHandler(

            IBaseRepository<RegDefi> defisRepository,

            IBaseRepository<RegPgdasd> pgdasdRepository,

            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,

            ICurrentUserService currentUser,

            Infrastructure.Context.CPGDbContext db,

            IMediator mediator)

        {

            _defisRepository = defisRepository;

            _pgdasdRepository = pgdasdRepository;

            _demonstrativoRepository = demonstrativoRepository;

            _currentUser = currentUser;

            _db = db;

            _mediator = mediator;

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

            var patrimonioLiquidoAcumulado = await ObterPatrimonioLiquidoFinalAnteriorAsync(

                idTenant,

                request.IdEmpresa,

                anos[0],

                cancellationToken);



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

                    var alertasAno = resultadoAno.Alertas;



                    var despesasDefis = GetValorDefis(defisRows, "Total de despesas no período", "despesas no período", "despesas no periodo");

                    if (despesasDefis == 0)

                    {

                        alertasAno.Add(

                            "DESPESAS_ZERADAS: O campo de despesas do período na DEFIS está zerado. " +

                            "Verifique se houve omissão na declaração."

                        );

                    }



                    var receitaDefis = GetValorDefis(

                        defisRows,

                        "Receita bruta",

                        "RECEITA BRUTA",

                        "Total de entradas no período",

                        "total de entradas");

                    var receitaPgdasd = pgdasdAno.Sum(x => x.Row.ReceitaBruta);

                    var deltaReceita = Math.Abs(receitaDefis - receitaPgdasd);

                    if (receitaDefis > 0 && receitaPgdasd > 0 && deltaReceita > 1m)

                    {

                        alertasAno.Add(

                            $"DIVERGENCIA_RECEITA: Receita na DEFIS (R$ {receitaDefis:N2}) difere do " +

                            $"somatório do PGDASD (R$ {receitaPgdasd:N2}). Diferença: R$ {deltaReceita:N2}. " +

                            "Verifique a consistência das declarações."

                        );

                    }



                    itensParaInserir.AddRange(BuildDefisAnual(idTenant, request.IdEmpresa, ano, defisRows, nowInsert));



                    itensParaInserir.AddRange(BuildPgdasd(

                        idTenant,

                        request.IdEmpresa,

                        ano,

                        pgdasdAno.Select(x => x.Row),

                        nowInsert));



                    var (sinteticos, patrimonioLiquidoFinal) = BuildSinteticosEcfAlinhados(

                        idTenant,

                        request.IdEmpresa,

                        ano,

                        defisRows,

                        pgdasdAno.Select(x => x.Row).ToList(),

                        patrimonioLiquidoAcumulado,

                        nowInsert,

                        alertasAno);



                    itensParaInserir.AddRange(sinteticos);

                    patrimonioLiquidoAcumulado = patrimonioLiquidoFinal;



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

                }

            }



            await _mediator.Send(new CalcIndicadoresCommand

            {

                IdEmpresa = request.IdEmpresa

            }, cancellationToken);



            await _mediator.Send(new CalcResultadosIndicesICPCommand

            {

                IdEmpresa = request.IdEmpresa

            }, cancellationToken);



            return response;

        }



        private async Task<decimal> ObterPatrimonioLiquidoFinalAnteriorAsync(

            long idTenant,

            long idEmpresa,

            int primeiroAnoProcessado,

            CancellationToken cancellationToken)

        {

            var registro = await _demonstrativoRepository

                .Query(dc => dc.IdTenant == idTenant &&

                             dc.IdEmpresa == idEmpresa &&

                             dc.Ano == primeiroAnoProcessado - 1 &&

                             dc.Codigo == CodigoPatrimonioLiquido &&

                             dc.PerApur == PerApurAnualEcd &&

                             dc.DeletedAt == null)

                .OrderByDescending(dc => dc.UpdatedAt)

                .FirstOrDefaultAsync(cancellationToken);



            return SaldoContabilHelper.SaldoAssinado(registro?.ValCtaRefFin, registro?.IndValCtaRefFin);

        }



        private static IEnumerable<DemonstrativoContabil> BuildDefisAnual(

            long idTenant,

            long idEmpresa,

            int ano,

            IEnumerable<RegDefi> defisRows,

            DateTime now)

        {

            decimal estoqueInicial = GetValorDefis(defisRows, "Estoque inicial", "estoque inic");

            decimal estoqueFinal = GetValorDefis(defisRows, "Estoque final");

            decimal caixaInicio = GetValorDefis(

                defisRows,

                "Saldo em caixa/banco no início",

                "caixa/banco no início",

                "caixa/banco no inicio");

            decimal caixaFinal = GetValorDefis(

                defisRows,

                "Saldo em caixa/banco no final",

                "caixa/banco no final");



            var dtIni = new DateOnly(ano, 1, 1);

            var dtFin = new DateOnly(ano, 12, 31);



            return new List<DemonstrativoContabil>

            {

                new()

                {

                    IdTenant = idTenant,

                    IdEmpresa = idEmpresa,

                    DtIni = dtIni,

                    DtIniApur = dtIni,

                    DtFinApur = dtFin,

                    PerApur = PerApurAnualEcd,

                    Ano = ano,

                    Codigo = "1.01.01",

                    Descricao = "DISPONIBILIDADES/CAIXAS E EQUIVALENTES",

                    Tipo = 'S',

                    Nivel = NivelDoCodigo("1.01.01"),

                    ValCtaRefIni = caixaInicio,

                    IndValCtaRefIni = 'D',

                    ValCtaRefDeb = null,

                    ValCtaRefCred = null,

                    ValCtaRefFin = caixaFinal,

                    IndValCtaRefFin = 'D',

                    TipoTrib = TipoTribSimplesNacional,

                    CreatedAt = now,

                    UpdatedAt = now,

                    DeletedAt = null

                },

                new()

                {

                    IdTenant = idTenant,

                    IdEmpresa = idEmpresa,

                    DtIni = dtIni,

                    DtIniApur = dtIni,

                    DtFinApur = dtFin,

                    PerApur = PerApurAnualEcd,

                    Ano = ano,

                    Codigo = "1.01.03",

                    Descricao = "ESTOQUES",

                    Tipo = 'S',

                    Nivel = NivelDoCodigo("1.01.03"),

                    ValCtaRefIni = estoqueInicial,

                    IndValCtaRefIni = 'D',

                    ValCtaRefDeb = null,

                    ValCtaRefCred = null,

                    ValCtaRefFin = estoqueFinal,

                    IndValCtaRefFin = 'D',

                    TipoTrib = TipoTribSimplesNacional,

                    CreatedAt = now,

                    UpdatedAt = now,

                    DeletedAt = null

                }

            };

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

                .Where(x => x.Dt.HasValue && x.Dt.Value.Year == ano)

                .ToList();



            var totalDas = parsed.Sum(x => x.Row.TotalDebito);



            var dtIniAno = new DateOnly(ano, 1, 1);

            var dtFinAno = new DateOnly(ano, 12, 31);



            return new List<DemonstrativoContabil>

            {

                new DemonstrativoContabil

                {

                    IdTenant = idTenant,

                    IdEmpresa = idEmpresa,

                    DtIni = dtIniAno,

                    DtIniApur = dtIniAno,

                    DtFinApur = dtFinAno,

                    PerApur = PerApurAnualEcd,

                    Ano = ano,

                    Codigo = "2.01.01.09.28",

                    Descricao = "OUTROS TRIBUTOS A RECOLHER - CIRCULANTE (DAS)",

                    Tipo = 'P',

                    Nivel = NivelDoCodigo("2.01.01.09.28"),

                    ValCtaRefIni = 0m,

                    IndValCtaRefIni = 'D',

                    ValCtaRefDeb = null,

                    ValCtaRefCred = null,

                    ValCtaRefFin = totalDas,

                    IndValCtaRefFin = 'D',

                    TipoTrib = TipoTribSimplesNacional,

                    CreatedAt = now,

                    UpdatedAt = now,

                    DeletedAt = null

                }

            };

        }



        /// <summary>Linha de resultado (DRE): A00, saldo em val_cta_ref_fin; val_cta_ref_ini sempre nulo.</summary>

        private static DemonstrativoContabil LinhaDreAnualA00Ecd(

            long idTenant,

            long idEmpresa,

            int ano,

            string codigo,

            string descricao,

            char tipo,

            decimal valCtaRefFin,

            char indValCtaRefFin,

            DateTime now)

        {

            var dtIni = new DateOnly(ano, 1, 1);

            var dtFin = new DateOnly(ano, 12, 31);

            return new DemonstrativoContabil

            {

                IdTenant = idTenant,

                IdEmpresa = idEmpresa,

                DtIni = dtIni,

                DtIniApur = dtIni,

                DtFinApur = dtFin,

                PerApur = PerApurAnualEcd,

                Ano = ano,

                Codigo = codigo,

                Descricao = descricao,

                Tipo = tipo,

                Nivel = NivelDoCodigo(codigo),

                ValCtaRefIni = null,

                IndValCtaRefIni = null,

                ValCtaRefDeb = null,

                ValCtaRefCred = null,

                ValCtaRefFin = valCtaRefFin,

                IndValCtaRefFin = indValCtaRefFin,

                TipoTrib = TipoTribSimplesNacional,

                CreatedAt = now,

                UpdatedAt = now,

                DeletedAt = null

            };

        }



        /// <summary>

        /// Totais sintéticos (tipo S, per_apur A00) e contas zeradas usadas em Indicadores.json,

        /// alinhados ao layout do modelo CAPAG (Simples Nacional / DEFIS + PGDASD).

        /// Passivo circulante = Ativo − Patrimônio líquido (plug de equilíbrio); DAS permanece em 2.01.01.09.28.

        /// </summary>

        private static (List<DemonstrativoContabil> Itens, decimal PatrimonioLiquidoFinal) BuildSinteticosEcfAlinhados(

            long idTenant,

            long idEmpresa,

            int ano,

            IEnumerable<RegDefi> defisRows,

            IReadOnlyCollection<RegPgdasd> pgdasdRowsAno,

            decimal patrimonioLiquidoInicial,

            DateTime now,

            List<string> alertas)

        {

            decimal caixaIni = GetValorDefis(

                defisRows,

                "Saldo em caixa/banco no início",

                "caixa/banco no início",

                "caixa/banco no inicio");

            decimal caixaFin = GetValorDefis(

                defisRows,

                "Saldo em caixa/banco no final",

                "caixa/banco no final");

            decimal estoqueIni = GetValorDefis(defisRows, "Estoque inicial", "estoque inic");

            decimal estoqueFin = GetValorDefis(defisRows, "Estoque final");

            decimal devVendas = GetValorDefis(

                defisRows,

                "Total de devoluções de vendas",

                "devoluções de vendas",

                "devolução de vendas");

            decimal despesas = GetValorDefis(

                defisRows,

                "Total de despesas no período",

                "despesas no período",

                "despesas no periodo");



            var totalReceita = 0m;

            var totalDas = 0m;

            foreach (var row in pgdasdRowsAno)

            {

                var dt = TryParsePeriodoPgdasd(row.Periodo);

                if (!dt.HasValue || dt.Value.Year != ano)

                    continue;

                totalReceita += row.ReceitaBruta;

                totalDas += row.TotalDebito;

            }



            var deducoesReceitaBruta = totalDas + devVendas;

            var receitaLiquida = Math.Max(0, totalReceita - deducoesReceitaBruta);

            var cmv = ResolverCmv(defisRows, receitaLiquida, ano, alertas);

            var lucroBruto = receitaLiquida - cmv;

            var lucroLiquido = lucroBruto - despesas;

            var (finRo, indRo) = NormalizarValorDreIndicador(lucroLiquido);

            var (fin3, ind3) = NormalizarValorDreIndicador(lucroLiquido);

            var (lbFin, lbInd) = NormalizarValorDreIndicador(lucroBruto);



            decimal acIni = caixaIni + estoqueIni;

            decimal acFin = caixaFin + estoqueFin;

            var ativoIni = acIni;

            var ativoFin = acFin;



            var patrimonioLiquidoFinal = patrimonioLiquidoInicial + lucroLiquido;

            var passivoCircFin = ativoFin - patrimonioLiquidoFinal;

            if (passivoCircFin < 0)

            {

                alertas.Add(

                    $"PASSIVO_NEGATIVO: Passivo circulante calculado para {ano} resultou em R$ {passivoCircFin:N2} " +

                    $"(Ativo R$ {ativoFin:N2}, PL R$ {patrimonioLiquidoFinal:N2}). Registrado como R$ 0,00."

                );

                passivoCircFin = 0;

            }



            var (plIniVal, plIniInd) = SaldoParaArmazenamento(patrimonioLiquidoInicial);

            var (plFinVal, plFinInd) = SaldoParaArmazenamento(patrimonioLiquidoFinal);



            var lista = new List<DemonstrativoContabil>

            {

                NovaLinhaBalancoZeradaA00(idTenant, idEmpresa, ano, "1.02", "ATIVO NÃO CIRCULANTE", now),

                NovaLinhaBalancoZeradaA00(idTenant, idEmpresa, ano, "2.01.01.03", "FORNECEDORES - CIRCULANTE", now),

                NovaLinhaBalancoZeradaA00(idTenant, idEmpresa, ano, "2.02", "PASSIVO NÃO-CIRCULANTE", now),



                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, "1.01", "ATIVO CIRCULANTE", 'S', acIni, 'D', acFin, 'D', now),

                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, "1", "ATIVO", 'S', ativoIni, 'D', ativoFin, 'D', now),



                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, CodigoPatrimonioLiquido, "PATRIMÔNIO LÍQUIDO", 'S', plIniVal, plIniInd, plFinVal, plFinInd, now),



                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, "2.01.01", "OBRIGAÇÕES CIRCULANTES", 'S', null, null, passivoCircFin, 'D', now),

                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, "2.01", "PASSIVO CIRCULANTE", 'S', null, null, passivoCircFin, 'D', now),

                NovaLinhaBalancoSinteticoA00(idTenant, idEmpresa, ano, "2", "PASSIVO", 'S', null, null, passivoCircFin, 'D', now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.01.01", "RECEITA BRUTA", 'S', totalReceita, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.01.02", "DEDUÇÕES DA RECEITA BRUTA", 'S', deducoesReceitaBruta, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.01", "RECEITA LÍQUIDA", 'S', receitaLiquida, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.03.01", "CUSTO DOS BENS E SERVIÇOS", 'S', cmv, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.03", "CUSTO DOS BENS E SERVIÇOS", 'S', cmv, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.02", "LUCRO BRUTO", 'S', lbFin, lbInd, now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.07.01.23", "DEPRECIAÇÃO/AMORTIZAÇÃO", 'S', 0m, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.07.01", "DESPESAS OPERACIONAIS", 'S', despesas, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.07", "TOTAL DESPESAS OPERACIONAIS", 'S', despesas, 'D', now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.05.01.99", "RECEITAS OPERACIONAIS", 'S', 0m, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.05.01.05", "RECEITAS FINANCEIRAS", 'S', 0m, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.05.01", "RECEITAS OPERACIONAIS E FINANCEIRAS", 'S', 0m, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.05", "RECEITAS", 'S', 0m, 'C', now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.09.01.08", "DESPESAS FINANCEIRAS", 'S', 0m, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.09.01.99", "OUTRAS DESPESAS NÃO OPERACIONAIS", 'S', 0m, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.09.01", "DESPESAS FINANCEIRAS E NÃO OPERACIONAIS", 'S', 0m, 'D', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.09", "DESPESAS FINANCEIRAS E NÃO OPERACIONAIS (GRUPO)", 'S', 0m, 'D', now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.11.01.02", "OUTRAS RECEITAS NÃO OPERACIONAIS", 'S', 0m, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.11.01", "OUTRAS RECEITAS NÃO OPERACIONAIS", 'S', 0m, 'C', now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01.11", "OUTRAS RECEITAS", 'S', 0m, 'C', now),



                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3.01.01", "RESULTADO OPERACIONAL", 'S', finRo, indRo, now),

                LinhaDreAnualA00Ecd(idTenant, idEmpresa, ano, "3", "RESULTADO LÍQUIDO DO PERÍODO", 'S', fin3, ind3, now)

            };



            foreach (var (codigo, descricao) in PlaceholdersContasIndicadores)

            {

                lista.Add(NovaLinhaBalancoZeradaA00(idTenant, idEmpresa, ano, codigo, descricao, now));

            }



            return (lista, patrimonioLiquidoFinal);

        }



        /// <summary>

        /// Prioriza CMV declarado na DEFIS; senão, fórmula por estoque. Alerta se houver divergência material.

        /// </summary>

        private static decimal ResolverCmv(

            IEnumerable<RegDefi> defisRows,

            decimal receitaLiquida,

            int ano,

            List<string> alertas)

        {

            var cmvDefis = GetValorDefis(

                defisRows,

                "CMV",

                "custo dos produtos vendidos",

                "custo dos bens e serviços",

                "custo das mercadorias");



            var estoqueIni = GetValorDefis(defisRows, "Estoque inicial", "estoque inic");

            var estoqueFin = GetValorDefis(defisRows, "Estoque final");

            var compras = GetValorDefis(

                defisRows,

                "Total de aquisições de mercadorias",

                "aquisições de mercadorias",

                "aquisicoes de mercadorias");

            var devCompras = GetValorDefis(

                defisRows,

                "Total de devoluções de compras",

                "devoluções de compras",

                "devolução de compras");



            var cmvEstoque = estoqueIni + compras - devCompras - estoqueFin;



            if (cmvDefis > 0)

            {

                if (cmvEstoque > 0 && Math.Abs(cmvDefis - cmvEstoque) > 1m)

                {

                    alertas.Add(

                        $"CMV_DIVERGENTE: CMV na DEFIS (R$ {cmvDefis:N2}) difere do calculado por estoque " +

                        $"(R$ {cmvEstoque:N2}) em {ano}. Utilizado o valor da DEFIS."

                    );

                }

                return cmvDefis;

            }



            if (cmvEstoque < 0)

            {

                alertas.Add(

                    $"CMV_NEGATIVO: O CMV calculado por estoque para {ano} resultou em R$ {cmvEstoque:N2}. " +

                    $"Fórmula: Est.Ini ({estoqueIni:N2}) + Compras ({compras:N2}) " +

                    $"- Dev.Compras ({devCompras:N2}) - Est.Final ({estoqueFin:N2}). " +

                    "Registrado como R$ 0,00."

                );

                return 0;

            }



            if (cmvEstoque == 0 && receitaLiquida > 0)

            {

                alertas.Add(

                    $"CMV_ZERADO: Não há CMV na DEFIS nem por estoque para {ano} com receita líquida positiva. " +

                    "Verifique os lançamentos de estoque e compras."

                );

            }



            return cmvEstoque;

        }



        private static readonly (string codigo, string descricao)[] PlaceholdersContasIndicadores =

        {

            ("1.01.02.02", "DUPLICATAS A RECEBER"),

            ("2.01.01.07", "OUTROS PASSIVOS CIRCULANTES"),

            ("2.01.01.09.09", "TRIBUTOS FEDERAIS A RECOLHER"),

            ("2.01.01.09.10", "TRIBUTOS ESTADUAIS A RECOLHER"),

            ("2.01.01.17.13", "OUTRAS OBRIGAÇÕES CIRCULANTES")

        };



        private static (decimal valor, char indicador) NormalizarValorDreIndicador(decimal saldo)

        {

            if (saldo >= 0)

                return (saldo, 'C');

            return (-saldo, 'D');

        }



        /// <summary>Converte saldo patrimonial (negativo = prejuízo) para valor absoluto + indicador D/C.</summary>

        private static (decimal valor, char indicador) SaldoParaArmazenamento(decimal saldoPatrimonial)

        {

            if (saldoPatrimonial >= 0)

                return (saldoPatrimonial, 'C');

            return (-saldoPatrimonial, 'D');

        }



        /// <summary>Balanço (patrimonial): val_cta_ref_ini obrigatoriamente preenchido (usa 0 quando não informado).</summary>

        private static DemonstrativoContabil NovaLinhaBalancoSinteticoA00(

            long idTenant,

            long idEmpresa,

            int ano,

            string codigo,

            string descricao,

            char tipo,

            decimal? valIni,

            char? indIni,

            decimal? valFin,

            char? indFin,

            DateTime now)

        {

            var dtIni = new DateOnly(ano, 1, 1);

            var dtFin = new DateOnly(ano, 12, 31);

            var iniValor = valIni ?? 0m;

            var iniIndicador = indIni ?? indFin ?? 'D';

            return new DemonstrativoContabil

            {

                IdTenant = idTenant,

                IdEmpresa = idEmpresa,

                DtIni = dtIni,

                DtIniApur = dtIni,

                DtFinApur = dtFin,

                PerApur = PerApurAnualEcd,

                Ano = ano,

                Codigo = codigo,

                Descricao = descricao,

                Tipo = tipo,

                Nivel = NivelDoCodigo(codigo),

                ValCtaRefIni = iniValor,

                IndValCtaRefIni = iniIndicador,

                ValCtaRefDeb = null,

                ValCtaRefCred = null,

                ValCtaRefFin = valFin,

                IndValCtaRefFin = indFin,

                TipoTrib = TipoTribSimplesNacional,

                CreatedAt = now,

                UpdatedAt = now,

                DeletedAt = null

            };

        }



        /// <summary>Conta de balanço com saldos zerados: val_cta_ref_ini e val_cta_ref_fin não nulos.</summary>

        private static DemonstrativoContabil NovaLinhaBalancoZeradaA00(

            long idTenant,

            long idEmpresa,

            int ano,

            string codigo,

            string descricao,

            DateTime now)

        {

            var dtIni = new DateOnly(ano, 1, 1);

            var dtFin = new DateOnly(ano, 12, 31);

            return new DemonstrativoContabil

            {

                IdTenant = idTenant,

                IdEmpresa = idEmpresa,

                DtIni = dtIni,

                DtIniApur = dtIni,

                DtFinApur = dtFin,

                PerApur = PerApurAnualEcd,

                Ano = ano,

                Codigo = codigo,

                Descricao = descricao,

                Tipo = 'S',

                Nivel = NivelDoCodigo(codigo),

                ValCtaRefIni = 0m,

                IndValCtaRefIni = null,

                ValCtaRefDeb = null,

                ValCtaRefCred = null,

                ValCtaRefFin = 0m,

                IndValCtaRefFin = null,

                TipoTrib = TipoTribSimplesNacional,

                CreatedAt = now,

                UpdatedAt = now,

                DeletedAt = null

            };

        }



        private static decimal GetValorDefis(IEnumerable<RegDefi> rows, params string[] chaves)

        {

            return rows

                .Where(r => r.Descricao != null &&

                            chaves.Any(chave => r.Descricao.Contains(chave, StringComparison.OrdinalIgnoreCase)))

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


