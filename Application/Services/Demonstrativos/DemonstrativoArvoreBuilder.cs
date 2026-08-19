using Application.Helpers;
using Application.Services.SinalContabil;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace Application.Services.Demonstrativos
{
    public class DemonstrativoArvoreBuilder
    {
        public List<LinhaDemonstrativoDto> Construir(
            IEnumerable<ContaAnoSaldo> saldos,
            IReadOnlyCollection<int> anos,
            ISet<string>? contasExcluidas = null)
        {
            var porCodigo = saldos
                .GroupBy(s => SaldoContabilHelper.NormalizarCodigo(s.Codigo))
                .ToDictionary(g => g.Key, g => g.ToList());

            var linhasPorCodigo = new Dictionary<string, LinhaDemonstrativoDto>(StringComparer.Ordinal);

            foreach (var (codigo, itens) in porCodigo)
            {
                var amostra = itens.OrderByDescending(x => x.Ano).First();
                var linha = new LinhaDemonstrativoDto
                {
                    Codigo = codigo,
                    Descricao = amostra.Descricao,
                    Nivel = amostra.Nivel != 0
                        ? amostra.Nivel
                        : (byte)codigo.Split('.', StringSplitOptions.RemoveEmptyEntries).Length,
                    Grupo = amostra.Grupo,
                    IndicadorDC = NormalizadorSinalService.ParseIndicador(amostra.Indicador),
                    Excluida = contasExcluidas != null && contasExcluidas.Contains(codigo)
                };

                foreach (var ano in anos)
                {
                    var itemAno = itens.FirstOrDefault(x => x.Ano == ano);
                    linha.Valores.PorAno[ano] = itemAno?.SaldoNormalizado ?? 0m;
                }

                if (linha.Excluida)
                {
                    foreach (var ano in anos)
                        linha.Valores.PorAno[ano] = 0m;
                }

                linhasPorCodigo[codigo] = linha;
            }

            var raizes = new List<LinhaDemonstrativoDto>();

            foreach (var linha in linhasPorCodigo.Values.OrderBy(l => l.Codigo, StringComparer.Ordinal))
            {
                var pai = SaldoContabilHelper.CodigoPai(linha.Codigo);
                if (pai != null && linhasPorCodigo.TryGetValue(pai, out var linhaPai))
                    linhaPai.Filhos.Add(linha);
                else
                    raizes.Add(linha);
            }

            foreach (var raiz in raizes)
                OrdenarFilhos(raiz);

            AtribuirPosicoes(raizes);
            return raizes;
        }

        public static void RecalcularSomaHierarquica(IEnumerable<LinhaDemonstrativoDto> raizes, IReadOnlyCollection<int> anos)
        {
            foreach (var raiz in raizes)
                SomarBottomUp(raiz, anos);
        }

        public static IEnumerable<LinhaDemonstrativoDto> Achatar(IEnumerable<LinhaDemonstrativoDto> raizes)
        {
            foreach (var linha in raizes)
            {
                yield return linha;
                foreach (var filho in Achatar(linha.Filhos))
                    yield return filho;
            }
        }

        public static LinhaDemonstrativoDto? Buscar(IEnumerable<LinhaDemonstrativoDto> raizes, string codigo)
        {
            return Achatar(raizes).FirstOrDefault(l => l.Codigo == codigo);
        }

        public static ValoresAnuaisDto SomarGrupo(IEnumerable<LinhaDemonstrativoDto> raizes, string prefixo, IReadOnlyCollection<int> anos)
        {
            var no = Buscar(raizes, prefixo);
            var resultado = new ValoresAnuaisDto();
            foreach (var ano in anos)
                resultado.PorAno[ano] = 0m;

            if (no == null)
                return resultado;

            if (no.Filhos.Count == 0)
                return CloneValores(no.Valores);

            foreach (var ano in anos)
                resultado.PorAno[ano] = SomarFolhas(no, ano);

            return resultado;
        }

        private static decimal SomarFolhas(LinhaDemonstrativoDto no, int ano)
        {
            if (no.Excluida)
                return 0m;

            if (no.Filhos.Count == 0)
                return no.Valores.Obter(ano);

            return no.Filhos.Sum(f => SomarFolhas(f, ano));
        }

        private static ValoresAnuaisDto SomarBottomUp(LinhaDemonstrativoDto no, IReadOnlyCollection<int> anos)
        {
            if (no.IsLinhaCalculada)
                return no.Valores;

            if (no.Filhos.Count == 0)
            {
                if (no.Excluida)
                {
                    foreach (var ano in anos)
                        no.Valores.PorAno[ano] = 0m;
                }
                return no.Valores;
            }

            foreach (var filho in no.Filhos)
                SomarBottomUp(filho, anos);

            if (SaldoContabilHelper.NormalizarCodigo(no.Codigo) == DreCapagConstants.CodigoResultadoLiquido)
                return no.Valores;

            if (no.Excluida)
            {
                foreach (var ano in anos)
                    no.Valores.PorAno[ano] = 0m;
                return no.Valores;
            }

            foreach (var ano in anos)
                no.Valores.PorAno[ano] = no.Filhos
                    .Where(f => !f.IsLinhaCalculada)
                    .Sum(f => f.Valores.Obter(ano));

            return no.Valores;
        }

        private static void OrdenarFilhos(LinhaDemonstrativoDto no)
        {
            no.Filhos = no.Filhos.OrderBy(f => f.Codigo, StringComparer.Ordinal).ToList();
            foreach (var filho in no.Filhos)
                OrdenarFilhos(filho);
        }

        private static void AtribuirPosicoes(IEnumerable<LinhaDemonstrativoDto> raizes)
        {
            var posicao = 0;
            foreach (var linha in Achatar(raizes))
                linha.Posicao = posicao++;
        }

        private static ValoresAnuaisDto CloneValores(ValoresAnuaisDto origem)
        {
            return new ValoresAnuaisDto
            {
                PorAno = new Dictionary<int, decimal>(origem.PorAno)
            };
        }
    }
}
