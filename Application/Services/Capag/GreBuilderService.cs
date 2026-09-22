using Application.Helpers;
using Application.Interfaces;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.Capag;
using Domain.Contracts.Demonstrativos;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Capag
{
    public class GreBuilderService
    {
        private readonly IDreCapagStructureProvider _dreStructureProvider;
        private readonly INormalizadorSinalService _normalizador;
        private readonly LucroBrutoCalculator _lucroBrutoCalculator;
        private readonly IBaseRepository<ContaGreInversao> _inversaoRepository;
        private readonly IBaseRepository<ContaGreManual> _manualRepository;
        private readonly IBaseRepository<ContaExclusaoConfig> _exclusaoRepository;

        public GreBuilderService(
            IDreCapagStructureProvider dreStructureProvider,
            INormalizadorSinalService normalizador,
            LucroBrutoCalculator lucroBrutoCalculator,
            IBaseRepository<ContaGreInversao> inversaoRepository,
            IBaseRepository<ContaGreManual> manualRepository,
            IBaseRepository<ContaExclusaoConfig> exclusaoRepository)
        {
            _dreStructureProvider = dreStructureProvider;
            _normalizador = normalizador;
            _lucroBrutoCalculator = lucroBrutoCalculator;
            _inversaoRepository = inversaoRepository;
            _manualRepository = manualRepository;
            _exclusaoRepository = exclusaoRepository;
        }

        public async Task<GreResultadoDto> ConstruirGre(long empresaId, CancellationToken cancellationToken = default)
        {
            var estrutura = await _dreStructureProvider.ObterEstruturaDreCapag(empresaId, cancellationToken);

            var (inversoes, justificativasInversao) = await CarregarInversoes(empresaId, cancellationToken);
            var manuais = await _manualRepository
                .Query(x => x.EmpresaId == empresaId)
                .ToListAsync(cancellationToken);
            var exclusoes = await _exclusaoRepository
                .Query(x => x.EmpresaId == empresaId)
                .ToListAsync(cancellationToken);

            foreach (var conta in estrutura.ContasFolha)
            {
                var linha = DemonstrativoArvoreBuilder.Buscar(estrutura.Linhas, conta.Codigo);
                if (linha == null || linha.Excluida)
                    continue;

                var valor = _normalizador.Normalizar(
                    conta.Codigo,
                    conta.SaldoBruto,
                    conta.Indicador);

                linha.Valores.PorAno[conta.Ano] = valor;
            }

            InserirManuais(estrutura.Linhas, estrutura.Anos, manuais);
            AplicarInversoesNasLinhas(estrutura.Linhas, inversoes, somenteComFilhos: false);
            DemonstrativoArvoreBuilder.RecalcularSomaHierarquica(estrutura.Linhas, estrutura.Anos);
            AplicarInversoesNasLinhas(estrutura.Linhas, inversoes, somenteComFilhos: true);
            AplicarMetadados(estrutura.Linhas, inversoes, justificativasInversao, exclusoes, manuais);
            _lucroBrutoCalculator.InserirNaArvore(estrutura.Linhas, estrutura.Anos);

            var lucroBruto = DemonstrativoArvoreBuilder.Buscar(estrutura.Linhas, DreCapagConstants.CodigoLucroBrutoCalculado);

            return new GreResultadoDto
            {
                IdEmpresa = empresaId,
                Anos = estrutura.Anos,
                Linhas = estrutura.Linhas,
                LucroBruto = lucroBruto,
                ContasManuais = manuais.Select(GreAjusteHelper.ToDto).ToList()
            };
        }

        private async Task<(Dictionary<string, Dictionary<int, bool>> Flags, Dictionary<string, string?> Justificativas)> CarregarInversoes(
            long empresaId,
            CancellationToken cancellationToken)
        {
            var rows = await _inversaoRepository
                .Query(x => x.EmpresaId == empresaId && x.Invertido)
                .ToListAsync(cancellationToken);

            var flags = rows
                .GroupBy(x => x.CodigoConta, StringComparer.Ordinal)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(x => x.Ano, x => true),
                    StringComparer.Ordinal);

            var justificativas = rows
                .Where(x => !string.IsNullOrWhiteSpace(x.Justificativa))
                .GroupBy(x => x.CodigoConta, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First().Justificativa, StringComparer.Ordinal);

            return (flags, justificativas);
        }

        private static void InserirManuais(
            List<LinhaDemonstrativoDto> raizes,
            IReadOnlyCollection<int> anos,
            IEnumerable<ContaGreManual> manuais)
        {
            foreach (var manual in manuais)
            {
                if (DemonstrativoArvoreBuilder.Buscar(raizes, manual.CodigoConta) != null)
                    continue;

                var valores = GreAjusteHelper.ResolverValoresManuais(
                    GreAjusteHelper.ParseValores(manual.ValoresJson),
                    anos,
                    manual.UsarMedia);

                var linha = new LinhaDemonstrativoDto
                {
                    Codigo = manual.CodigoConta,
                    Descricao = manual.Descricao,
                    IsManual = true,
                    Justificativa = manual.Justificativa,
                    Grupo = manual.Tipo == GreAjusteHelper.TipoDespesa
                        ? GrupoContabil.Despesa
                        : GrupoContabil.Receita,
                    Nivel = 1,
                    Valores = new ValoresAnuaisDto { PorAno = valores }
                };

                var pai = !string.IsNullOrWhiteSpace(manual.CodigoPai)
                    ? DemonstrativoArvoreBuilder.Buscar(raizes, SaldoContabilHelper.NormalizarCodigo(manual.CodigoPai))
                    : null;

                if (pai != null)
                {
                    linha.Nivel = (byte)(pai.Nivel + 1);
                    pai.Filhos.Add(linha);
                }
                else
                {
                    raizes.Add(linha);
                }
            }
        }

        private static void AplicarInversoesNasLinhas(
            IEnumerable<LinhaDemonstrativoDto> raizes,
            Dictionary<string, Dictionary<int, bool>> inversoes,
            bool somenteComFilhos)
        {
            foreach (var linha in DemonstrativoArvoreBuilder.Achatar(raizes))
            {
                if (linha.IsLinhaCalculada)
                    continue;

                var temFilhos = linha.Filhos.Count > 0;
                if (somenteComFilhos != temFilhos)
                    continue;

                if (!inversoes.TryGetValue(linha.Codigo, out var porAno))
                    continue;

                foreach (var (ano, invertido) in porAno)
                {
                    if (!invertido)
                        continue;
                    linha.Valores.PorAno[ano] = GreAjusteHelper.AplicarInversao(linha.Valores.Obter(ano), true);
                }
            }
        }

        private static void AplicarMetadados(
            IEnumerable<LinhaDemonstrativoDto> raizes,
            Dictionary<string, Dictionary<int, bool>> inversoes,
            Dictionary<string, string?> justificativasInversao,
            IReadOnlyCollection<ContaExclusaoConfig> exclusoes,
            IReadOnlyCollection<ContaGreManual> manuais)
        {
            var justificativaExclusao = exclusoes
                .Where(x => !string.IsNullOrWhiteSpace(x.Justificativa))
                .ToDictionary(x => x.CodigoConta, x => x.Justificativa, StringComparer.Ordinal);

            var justificativaManual = manuais
                .Where(x => !string.IsNullOrWhiteSpace(x.Justificativa))
                .ToDictionary(x => x.CodigoConta, x => x.Justificativa, StringComparer.Ordinal);

            foreach (var linha in DemonstrativoArvoreBuilder.Achatar(raizes))
            {
                if (inversoes.TryGetValue(linha.Codigo, out var flags))
                    linha.ValoresSinalInvertido = flags;

                linha.Justificativa = GreAjusteHelper.PrimeiraJustificativa(
                    linha.Justificativa,
                    justificativaExclusao.GetValueOrDefault(linha.Codigo),
                    justificativasInversao.GetValueOrDefault(linha.Codigo),
                    justificativaManual.GetValueOrDefault(linha.Codigo));
            }
        }
    }
}
