using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Demonstrativos
{
    /// <summary>
    /// Lê demonstrativos e devolve saldos já normalizados pelo serviço central de sinal.
    /// </summary>
    public class DemonstrativoLeituraService
    {
        private readonly IBaseRepository<DemonstrativoContabil> _repository;
        private readonly INormalizadorSinalService _normalizador;
        private readonly IGrupoContabilResolver _grupoResolver;

        public DemonstrativoLeituraService(
            IBaseRepository<DemonstrativoContabil> repository,
            INormalizadorSinalService normalizador,
            IGrupoContabilResolver grupoResolver)
        {
            _repository = repository;
            _normalizador = normalizador;
            _grupoResolver = grupoResolver;
        }

        public async Task<List<ContaAnoSaldo>> ObterSaldosNormalizadosAsync(
            long empresaId,
            IEnumerable<int>? anosFiltro = null,
            CancellationToken cancellationToken = default)
        {
            var anos = anosFiltro?.ToHashSet();
            var anosConsulta = anos == null
                ? null
                : anos.Concat(anos.Select(a => a - 1)).ToHashSet();

            var query = _repository.Query(x =>
                x.IdEmpresa == empresaId &&
                x.DeletedAt == null &&
                !string.IsNullOrWhiteSpace(x.Codigo) &&
                (anosConsulta == null || anosConsulta.Contains(x.Ano)));

            var registros = await query.ToListAsync(cancellationToken);

            var porCodigoAno = registros
                .GroupBy(x => (Codigo: x.Codigo, x.Ano))
                .ToDictionary(g => g.Key, g => g.ToList());

            var resultado = new List<ContaAnoSaldo>();

            foreach (var (chave, items) in porCodigoAno)
            {
                if (anos != null && !anos.Contains(chave.Ano))
                    continue;
                var isDre = items.All(x => x.ValCtaRefIni == null);
                var isTrimestral = DemonstrativoPeriodoHelper.EhTrimestral(items.Select(x => x.PerApur));
                var fonte = DemonstrativoPeriodoHelper.FechamentoDoExercicio(items, x => x.PerApur);

                decimal saldoBruto;
                char? indicador;
                decimal normalizado;
                var codigoSinal = CodigoParaSinal(chave.Codigo, isDre);

                if (isDre && isTrimestral)
                {
                    var consolidado = ConsolidarDreTrimestral(chave.Codigo, items, _normalizador);
                    resultado.Add(Criar(
                        fonte ?? items[0],
                        chave.Ano,
                        isDre,
                        consolidado.Normalizado,
                        consolidado.Indicador,
                        consolidado.Magnitude));
                    continue;
                }

                saldoBruto = fonte?.ValCtaRefFin ?? 0m;
                indicador = fonte?.IndValCtaRefFin;
                normalizado = _normalizador.Normalizar(codigoSinal, saldoBruto, indicador);
                decimal? iniNorm = 0m;
                decimal? iniBruto = null;
                char? iniInd = null;

                if (!isDre && porCodigoAno.TryGetValue((chave.Codigo, chave.Ano - 1), out var anoAnterior))
                {
                    var fechamentoAnterior = DemonstrativoPeriodoHelper.FechamentoParaSaldoInicial(
                        anoAnterior, x => x.PerApur);
                    if (fechamentoAnterior != null)
                    {
                        iniBruto = fechamentoAnterior.ValCtaRefFin;
                        iniInd = fechamentoAnterior.IndValCtaRefFin;
                        iniNorm = _normalizador.Normalizar(codigoSinal, iniBruto, iniInd);
                    }
                }

                resultado.Add(Criar(fonte ?? items[0], chave.Ano, isDre, normalizado, indicador, saldoBruto, iniBruto, iniInd, iniNorm));
            }

            return resultado;
        }

        /// <summary>
        /// DRE trimestral: soma T01…T04 com C = + e D = − em cada período.
        /// O sinal da soma define C ou D do resultado.
        /// </summary>
        public static (decimal Normalizado, char? Indicador, decimal Magnitude) ConsolidarDreTrimestral(
            string codigo,
            IReadOnlyList<DemonstrativoContabil> items,
            INormalizadorSinalService normalizador)
        {
            var trimestrais = items
                .Where(x => x.PerApur != null && x.PerApur.StartsWith("T0", StringComparison.Ordinal))
                .ToList();
            var fonte = trimestrais.Count > 0 ? trimestrais : items.ToList();

            var soma = fonte.Sum(x => normalizador.Normalizar(codigo, x.ValCtaRefFin, x.IndValCtaRefFin));
            var indicadorSoma = soma < 0m ? 'D' : 'C';
            return (soma, indicadorSoma, Math.Abs(soma));
        }

        private ContaAnoSaldo Criar(
            DemonstrativoContabil fonte,
            int ano,
            bool isDre,
            decimal normalizado,
            char? indicador,
            decimal saldoBruto,
            decimal? iniBruto = null,
            char? iniInd = null,
            decimal? iniNorm = null)
        {
            return new ContaAnoSaldo
            {
                Codigo = fonte.Codigo,
                Descricao = fonte.Descricao,
                Nivel = fonte.Nivel,
                Tipo = fonte.Tipo,
                Ano = ano,
                SaldoBruto = saldoBruto,
                Indicador = indicador,
                SaldoNormalizado = normalizado,
                SaldoInicialBruto = iniBruto,
                IndicadorInicial = iniInd,
                SaldoInicialNormalizado = iniNorm,
                Grupo = GrupoDaConta(fonte.Codigo, isDre),
                IsDre = isDre
            };
        }

        private GrupoContabil GrupoDaConta(string codigo, bool isDre)
        {
            if (!isDre && PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(codigo))
                return GrupoContabil.PatrimonioLiquido;

            return _grupoResolver.Resolver(codigo);
        }

        private static string CodigoParaSinal(string codigo, bool isDre) =>
            !isDre && PatrimonioLiquidoCodigoMapper.EhCodigoBalanco(codigo)
                ? PatrimonioLiquidoCodigoMapper.ParaCodigoFormula(codigo)
                : codigo;
    }
}
