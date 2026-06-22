namespace Application.Helpers
{
    public static class DemonstrativosAnosHelper
    {
        public const int QuantidadeAnosCalculo = 3;

        public static IReadOnlyList<int> ObterJanelaUltimosAnos(IEnumerable<int> anosDisponiveis, int quantidade = QuantidadeAnosCalculo)
        {
            var anos = anosDisponiveis.Distinct().ToList();
            if (anos.Count == 0)
                return Array.Empty<int>();

            var ultimoAno = anos.Max();
            return Enumerable.Range(ultimoAno - quantidade + 1, quantidade).ToList();
        }
    }
}
