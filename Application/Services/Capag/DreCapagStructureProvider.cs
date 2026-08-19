using Application.Helpers;
using Application.Interfaces;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Capag
{
    public class DreCapagStructureProvider : IDreCapagStructureProvider
    {
        private readonly DemonstrativoLeituraService _leitura;
        private readonly DemonstrativoArvoreBuilder _arvoreBuilder;
        private readonly IBaseRepository<ContaExclusaoConfig> _exclusaoRepository;

        public DreCapagStructureProvider(
            DemonstrativoLeituraService leitura,
            DemonstrativoArvoreBuilder arvoreBuilder,
            IBaseRepository<ContaExclusaoConfig> exclusaoRepository)
        {
            _leitura = leitura;
            _arvoreBuilder = arvoreBuilder;
            _exclusaoRepository = exclusaoRepository;
        }

        public async Task<DreCapagEstrutura> ObterEstruturaDreCapag(long empresaId, CancellationToken cancellationToken = default)
        {
            var saldos = await _leitura.ObterSaldosNormalizadosAsync(empresaId, cancellationToken: cancellationToken);
            var dre = saldos
                .Where(s =>
                {
                    if (!s.IsDre && PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(s.Codigo))
                        return false;

                    return s.IsDre
                        || SaldoContabilHelper.NormalizarCodigo(s.Codigo).StartsWith("3", StringComparison.Ordinal);
                })
                .ToList();

            var anos = dre.Select(s => s.Ano).Distinct().OrderBy(a => a).ToList();
            var excluidas = await ObterExcluidas(empresaId, cancellationToken);

            var linhas = _arvoreBuilder.Construir(dre, anos, excluidas);
            OrdenarAPartirDaReceitaBruta(linhas);

            var folhas = dre
                .Where(s => !linhasPossuiFilhos(linhas, s.Codigo))
                .Select(s => new ContaAnoSaldoRaw
                {
                    Codigo = s.Codigo,
                    Descricao = s.Descricao,
                    Ano = s.Ano,
                    SaldoBruto = s.SaldoBruto,
                    Indicador = s.Indicador,
                    ValorNormalizado = excluidas.Contains(s.Codigo) ? 0m : s.SaldoNormalizado
                })
                .ToList();

            return new DreCapagEstrutura
            {
                EmpresaId = empresaId,
                Anos = anos,
                Linhas = linhas,
                ContasFolha = folhas,
                ContasExcluidas = excluidas
            };
        }

        private async Task<HashSet<string>> ObterExcluidas(long empresaId, CancellationToken cancellationToken)
        {
            var rows = await _exclusaoRepository
                .Query(x => x.EmpresaId == empresaId && x.Excluida)
                .Select(x => x.CodigoConta)
                .ToListAsync(cancellationToken);

            return rows.ToHashSet(StringComparer.Ordinal);
        }

        private static bool linhasPossuiFilhos(IEnumerable<Domain.Contracts.Demonstrativos.LinhaDemonstrativoDto> raizes, string codigo)
        {
            var no = DemonstrativoArvoreBuilder.Buscar(raizes, codigo);
            return no != null && no.Filhos.Count > 0;
        }

        /// <summary>
        /// Garante que a árvore comece pela receita bruta, preservando a ordem original da DRE.
        /// </summary>
        private static void OrdenarAPartirDaReceitaBruta(List<Domain.Contracts.Demonstrativos.LinhaDemonstrativoDto> linhas)
        {
            linhas.Sort((a, b) =>
            {
                var aReceita = a.Codigo == DreCapagConstants.CodigoReceitaBruta || a.Codigo.StartsWith("3.01.01.01", StringComparison.Ordinal) ? 0 : 1;
                var bReceita = b.Codigo == DreCapagConstants.CodigoReceitaBruta || b.Codigo.StartsWith("3.01.01.01", StringComparison.Ordinal) ? 0 : 1;
                var cmp = aReceita.CompareTo(bReceita);
                return cmp != 0 ? cmp : string.Compare(a.Codigo, b.Codigo, StringComparison.Ordinal);
            });
        }
    }
}
