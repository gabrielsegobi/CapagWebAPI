using Application.Helpers;
using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;
using Domain.Enums;

namespace Application.Services.Capag
{
    public class LucroBrutoCalculator
    {
        public LinhaDemonstrativoDto Calcular(IReadOnlyList<LinhaDemonstrativoDto> linhas, IReadOnlyCollection<int> anos)
        {
            var receitaLiquida = DemonstrativoArvoreBuilder.Buscar(linhas, DreCapagConstants.CodigoReceitaLiquida)
                ?? DemonstrativoArvoreBuilder.Buscar(linhas, DreCapagConstants.CodigoReceitaLiquidaAlias);

            var valoresReceita = receitaLiquida?.Valores ?? new ValoresAnuaisDto();
            var valoresCustos = DemonstrativoArvoreBuilder.SomarGrupo(linhas, DreCapagConstants.CodigoGrupoCustos, anos);

            var linha = new LinhaDemonstrativoDto
            {
                Codigo = DreCapagConstants.CodigoLucroBrutoCalculado,
                Descricao = DreCapagConstants.DescricaoLucroBruto,
                IsLinhaCalculada = true,
                Grupo = GrupoContabil.Receita,
                IndicadorDC = IndicadorDC.Credito,
                Nivel = 4
            };

            // 3.01.01.03 (pai) é sempre positiva; folhas ainda podem vir com sinal D/C.
            foreach (var ano in anos)
                linha.Valores.PorAno[ano] =
                    valoresReceita.Obter(ano) - Math.Abs(valoresCustos.Obter(ano));

            linha.Posicao = PosicaoAposGrupo(linhas, DreCapagConstants.CodigoGrupoCustos);
            return linha;
        }

        public void InserirNaArvore(List<LinhaDemonstrativoDto> raizes, IReadOnlyCollection<int> anos)
        {
            var linha = Calcular(raizes, anos);
            var grupoCustos = DemonstrativoArvoreBuilder.Buscar(raizes, DreCapagConstants.CodigoGrupoCustos);
            var paiCodigo = grupoCustos != null
                ? SaldoContabilHelper.CodigoPai(grupoCustos.Codigo)
                : "3.01.01";

            var pai = paiCodigo != null ? DemonstrativoArvoreBuilder.Buscar(raizes, paiCodigo) : null;

            if (pai != null)
            {
                var idx = pai.Filhos.FindIndex(f => f.Codigo == DreCapagConstants.CodigoGrupoCustos);
                if (idx >= 0)
                    pai.Filhos.Insert(idx + 1, linha);
                else
                    pai.Filhos.Add(linha);
            }
            else
            {
                raizes.Add(linha);
            }

            ReatribuirPosicoes(raizes);
            linha.Posicao = PosicaoAposGrupo(raizes, DreCapagConstants.CodigoGrupoCustos);
        }

        public static int PosicaoAposGrupo(IEnumerable<LinhaDemonstrativoDto> raizes, string codigoGrupo)
        {
            var flat = DemonstrativoArvoreBuilder.Achatar(raizes).ToList();
            var last = -1;
            for (var i = 0; i < flat.Count; i++)
            {
                if (SaldoContabilHelper.EhCodigoOuFilho(flat[i].Codigo, codigoGrupo))
                    last = i;
            }

            return last >= 0 ? last + 1 : flat.Count;
        }

        private static void ReatribuirPosicoes(IEnumerable<LinhaDemonstrativoDto> raizes)
        {
            var posicao = 0;
            foreach (var linha in DemonstrativoArvoreBuilder.Achatar(raizes))
                linha.Posicao = posicao++;
        }
    }
}
