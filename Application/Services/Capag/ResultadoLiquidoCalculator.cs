using Application.Services.Demonstrativos;
using Domain.Constants;
using Domain.Contracts.Demonstrativos;

namespace Application.Services.Capag
{
    /// <summary>
    /// Resultado Líquido do Período isolado no mesmo pipeline de normalização do Balanço:
    /// usa o D/C da própria conta "3", nunca a soma crua dos filhos.
    /// </summary>
    public class ResultadoLiquidoCalculator
    {
        public ValoresAnuaisDto Calcular(IReadOnlyList<LinhaDemonstrativoDto> linhas, IReadOnlyCollection<int> anos)
        {
            var conta = DemonstrativoArvoreBuilder.Buscar(linhas, DreCapagConstants.CodigoResultadoLiquido);
            if (conta != null && !conta.IsLinhaCalculada)
                return new ValoresAnuaisDto { PorAno = new Dictionary<int, decimal>(conta.Valores.PorAno) };

            return CalcularPorNatureza(linhas, anos);
        }

        public ValoresAnuaisDto CalcularPorNatureza(IEnumerable<LinhaDemonstrativoDto> linhas, IReadOnlyCollection<int> anos)
        {
            var folhas = DemonstrativoArvoreBuilder.Achatar(linhas)
                .Where(l => l.Filhos.Count == 0 && !l.IsLinhaCalculada && !l.Excluida)
                .ToList();

            var resultado = new ValoresAnuaisDto();
            foreach (var ano in anos)
            {
                decimal soma = 0m;
                foreach (var folha in folhas)
                    soma += folha.Valores.Obter(ano);

                resultado.PorAno[ano] = soma;
            }

            return resultado;
        }
    }
}
