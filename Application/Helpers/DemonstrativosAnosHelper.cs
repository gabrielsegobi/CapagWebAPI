namespace Application.Helpers
{
    public static class DemonstrativosAnosHelper
    {
        public const int QuantidadeAnosCalculo = 4;

        /// <summary>
        /// Janela contígua de N anos terminando no maior ano disponível.
        /// Usada no cálculo de indicadores a partir dos demonstrativos já gravados.
        /// </summary>
        public static IReadOnlyList<int> ObterJanelaUltimosAnos(IEnumerable<int> anosDisponiveis, int quantidade = QuantidadeAnosCalculo)
        {
            var anos = anosDisponiveis.Distinct().ToList();
            if (anos.Count == 0)
                return Array.Empty<int>();

            var ultimoAno = anos.Max();
            return Enumerable.Range(ultimoAno - quantidade + 1, quantidade).ToList();
        }

        /// <summary>
        /// Os N anos mais recentes presentes na lista (sem inventar anos intermediários).
        /// Usada na importação GMaster a partir dos anos de tributação.
        /// </summary>
        public static IReadOnlyList<int> ObterAnosMaisRecentes(IEnumerable<int> anosDisponiveis, int quantidade = QuantidadeAnosCalculo)
        {
            return anosDisponiveis
                .Where(a => a > 0)
                .Distinct()
                .OrderByDescending(a => a)
                .Take(quantidade)
                .OrderBy(a => a)
                .ToList();
        }

        /// <summary>
        /// Anos de importação: os N mais recentes da tributação, mais o ano imediatamente
        /// anterior ao mais antigo deles quando esse ano também existir na tributação.
        /// O ano extra alimenta o saldo inicial [I] do primeiro exercício da janela de cálculo
        /// sem alterar a janela de indicadores (sempre centrada no Max).
        /// </summary>
        public static IReadOnlyList<int> ObterAnosImportacaoComAnterior(
            IEnumerable<int> anosTributacao,
            int quantidade = QuantidadeAnosCalculo)
        {
            var trib = anosTributacao.Where(a => a > 0).Distinct().ToHashSet();
            var principais = ObterAnosMaisRecentes(trib, quantidade).ToList();
            if (principais.Count == 0)
                return principais;

            var anterior = principais[0] - 1;
            if (trib.Contains(anterior) && !principais.Contains(anterior))
                principais.Insert(0, anterior);

            return principais;
        }
    }
}
