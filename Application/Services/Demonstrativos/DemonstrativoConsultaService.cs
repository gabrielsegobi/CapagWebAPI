using Application.Helpers;
using Application.Interfaces;
using Application.Services.Capag;
using Application.Services.SinalContabil;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;

namespace Application.Services.Demonstrativos
{
    /// <summary>
    /// Fonte única das árvores de <c>/api/balanco</c> e <c>/api/dre</c>.
    /// Indicadores e ICP leem os mesmos valores que essas APIs devolvem.
    /// </summary>
    public class DemonstrativoConsultaService
    {
        private readonly DemonstrativoLeituraService _leitura;
        private readonly DemonstrativoArvoreBuilder _arvoreBuilder;
        private readonly IDreCapagStructureProvider _dreProvider;
        private readonly LucroBrutoCalculator _lucroBrutoCalculator;
        private readonly ResultadoLiquidoCalculator _resultadoLiquidoCalculator;

        public DemonstrativoConsultaService(
            DemonstrativoLeituraService leitura,
            DemonstrativoArvoreBuilder arvoreBuilder,
            IDreCapagStructureProvider dreProvider,
            LucroBrutoCalculator lucroBrutoCalculator,
            ResultadoLiquidoCalculator resultadoLiquidoCalculator)
        {
            _leitura = leitura;
            _arvoreBuilder = arvoreBuilder;
            _dreProvider = dreProvider;
            _lucroBrutoCalculator = lucroBrutoCalculator;
            _resultadoLiquidoCalculator = resultadoLiquidoCalculator;
        }

        public async Task<DemonstrativoArvoreDto> ObterBalanco(long empresaId, CancellationToken cancellationToken = default)
        {
            var saldos = await _leitura.ObterSaldosNormalizadosAsync(empresaId, cancellationToken: cancellationToken);
            var balanco = saldos.Where(s => !s.IsDre).ToList();
            var anos = balanco.Select(s => s.Ano).Distinct().OrderBy(a => a).ToList();
            var linhas = _arvoreBuilder.Construir(balanco, anos);

            return new DemonstrativoArvoreDto
            {
                IdEmpresa = empresaId,
                Anos = anos,
                Linhas = linhas
            };
        }

        public async Task<DemonstrativoArvoreDto> ObterDre(long empresaId, CancellationToken cancellationToken = default)
        {
            var estrutura = await _dreProvider.ObterEstruturaDreCapag(empresaId, cancellationToken);

            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(estrutura.Linhas, estrutura.Anos);
            _lucroBrutoCalculator.InserirNaArvore(estrutura.Linhas, estrutura.Anos);

            var linhaRl = DemonstrativoArvoreBuilder.Buscar(estrutura.Linhas, DreCapagConstants.CodigoResultadoLiquido);
            var resultadoLiquido = linhaRl != null
                ? new ValoresAnuaisDto { PorAno = new Dictionary<int, decimal>(linhaRl.Valores.PorAno) }
                : _resultadoLiquidoCalculator.Calcular(estrutura.Linhas, estrutura.Anos);

            return new DemonstrativoArvoreDto
            {
                IdEmpresa = empresaId,
                Anos = estrutura.Anos,
                Linhas = estrutura.Linhas,
                ResultadoLiquido = resultadoLiquido
            };
        }

        /// <summary>
        /// Plano codigo→valor por ano, alinhado a <c>/api/balanco</c> e <c>/api/dre</c>.
        /// PL e resultado líquido entram com o D/C da ECF (C = +, D = −).
        /// <c>{codigo}[I]</c> é o saldo final do ano anterior (T04 trimestral, A00 anual); sem o ano N-1, 0.
        /// </summary>
        public async Task<Dictionary<int, Dictionary<string, decimal>>> ObterValoresComoNasApis(
            long empresaId,
            CancellationToken cancellationToken = default)
        {
            var balanco = await ObterBalanco(empresaId, cancellationToken);
            var dre = await ObterDre(empresaId, cancellationToken);
            var saldos = await _leitura.ObterSaldosNormalizadosAsync(empresaId, cancellationToken: cancellationToken);
            return MontarMapaValores(balanco, dre, saldos);
        }

        public static Dictionary<int, Dictionary<string, decimal>> MontarMapaValores(
            DemonstrativoArvoreDto balanco,
            DemonstrativoArvoreDto dre,
            IEnumerable<ContaAnoSaldo> saldos)
        {
            var mapa = new Dictionary<int, Dictionary<string, decimal>>();

            Mesclar(mapa, balanco.Linhas);
            AliasPlBalancoNasFormulas(mapa, balanco.Linhas);
            Mesclar(mapa, dre.Linhas);

            foreach (var saldo in saldos)
            {
                var codigo = SaldoContabilHelper.NormalizarCodigo(saldo.Codigo);
                if (string.IsNullOrEmpty(codigo))
                    continue;

                var chave = !saldo.IsDre && PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(codigo)
                    ? PatrimonioLiquidoCodigoMapper.ParaCodigoFormula(codigo)
                    : codigo;

                GarantirAno(mapa, saldo.Ano)[chave] = NormalizadorSinalService.AplicarIndicadorEcf(
                    chave, saldo.SaldoNormalizado, saldo.Indicador);
            }

            foreach (var saldo in saldos.Where(s => !s.IsDre))
            {
                var codigo = SaldoContabilHelper.NormalizarCodigo(saldo.Codigo);
                if (string.IsNullOrEmpty(codigo))
                    continue;

                var chave = PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(codigo)
                    ? PatrimonioLiquidoCodigoMapper.ParaCodigoFormula(codigo)
                    : codigo;

                GarantirAno(mapa, saldo.Ano)[$"{chave}[I]"] =
                    NormalizadorSinalService.AplicarIndicadorEcf(
                        chave, saldo.SaldoInicialNormalizado ?? 0m, saldo.IndicadorInicial);
            }

            CompletarPaisAusentes(mapa);
            return mapa;
        }

        /// <summary>
        /// PL do balanço como <c>3</c>/<c>3.*</c> também fica disponível como <c>2.03</c>/<c>2.03.*</c>
        /// para as fórmulas, antes da DRE sobrescrever a chave <c>3</c>.
        /// </summary>
        private static void AliasPlBalancoNasFormulas(
            Dictionary<int, Dictionary<string, decimal>> mapa,
            IEnumerable<LinhaDemonstrativoDto> linhasBalanco)
        {
            foreach (var linha in DemonstrativoArvoreBuilder.Achatar(linhasBalanco))
            {
                if (!PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(linha.Codigo))
                    continue;

                var chaveFormula = PatrimonioLiquidoCodigoMapper.ParaCodigoFormula(linha.Codigo);
                foreach (var (ano, valor) in linha.Valores.PorAno)
                    GarantirAno(mapa, ano)[chaveFormula] = valor;
            }
        }

        /// <summary>
        /// Fórmulas usam códigos-pai (ex.: <c>3.01.01.03</c>). Se o ECD só tem a analítica,
        /// preenche o pai ausente com a soma das folhas descendentes.
        /// </summary>
        private static void CompletarPaisAusentes(Dictionary<int, Dictionary<string, decimal>> mapa)
        {
            foreach (var porCodigo in mapa.Values)
            {
                var chaves = porCodigo.Keys.ToList();
                var pendentes = new HashSet<string>(StringComparer.Ordinal);

                foreach (var chave in chaves)
                {
                    if (EhChaveAbertura(chave))
                        continue;

                    var codigo = SaldoContabilHelper.NormalizarCodigo(chave);

                    for (var pai = SaldoContabilHelper.CodigoPai(codigo); pai != null; pai = SaldoContabilHelper.CodigoPai(pai))
                    {
                        if (pai == DreCapagConstants.CodigoResultadoLiquido && porCodigo.ContainsKey(pai))
                            continue;

                        if (!porCodigo.TryGetValue(pai, out var atual) || atual == 0m)
                            pendentes.Add(pai);
                    }
                }

                foreach (var chavePai in pendentes.OrderByDescending(c => c.Length))
                    porCodigo[chavePai] = SomarFolhasDescendentes(porCodigo, chavePai, abertura: false);
            }
        }

        private static decimal SomarFolhasDescendentes(
            Dictionary<string, decimal> porCodigo,
            string codigoPai,
            bool abertura)
        {
            var prefixo = codigoPai + ".";
            var candidatos = new List<(string Codigo, decimal Valor)>();
            foreach (var (chave, valor) in porCodigo)
            {
                if (EhChaveAbertura(chave) != abertura)
                    continue;

                var codigo = CodigoSemAbertura(chave);
                if (codigo.StartsWith(prefixo, StringComparison.Ordinal))
                    candidatos.Add((codigo, valor));
            }

            decimal soma = 0m;
            foreach (var (codigo, valor) in candidatos)
            {
                if (candidatos.Any(o => o.Codigo.StartsWith(codigo + ".", StringComparison.Ordinal)))
                    continue;

                soma += valor;
            }

            return soma;
        }

        private static bool EhChaveAbertura(string chave) =>
            chave.EndsWith("[I]", StringComparison.Ordinal);

        private static string CodigoSemAbertura(string chave) =>
            EhChaveAbertura(chave) ? chave[..^3] : chave;

        private static void Mesclar(
            Dictionary<int, Dictionary<string, decimal>> mapa,
            IEnumerable<LinhaDemonstrativoDto> linhas)
        {
            foreach (var linha in DemonstrativoArvoreBuilder.Achatar(linhas))
            {
                foreach (var (ano, valor) in linha.Valores.PorAno)
                    GarantirAno(mapa, ano)[linha.Codigo] = valor;
            }
        }

        private static Dictionary<string, decimal> GarantirAno(
            Dictionary<int, Dictionary<string, decimal>> mapa,
            int ano)
        {
            if (!mapa.TryGetValue(ano, out var porCodigo))
            {
                porCodigo = new Dictionary<string, decimal>(StringComparer.Ordinal);
                mapa[ano] = porCodigo;
            }

            return porCodigo;
        }
    }
}
