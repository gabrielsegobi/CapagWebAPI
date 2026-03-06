namespace Domain.Resources
{
    public static class IndicadorDescricaoMapper
    {
        private static readonly Dictionary<string, string> _descricaoIndicadores = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Liquidez Geral", "Mede a capacidade da empresa de pagar todas as dívidas, de curto e longo prazo." },
            { "Liquidez Seca", "Avalia a capacidade de quitar dívidas de curto prazo sem depender dos estoques." },
            { "Liquidez Imediata", "Mostra quanto das dívidas de curto prazo pode ser paga com o caixa disponível." },

            { "Índice de Endividamento Geral", "Indica a proporção do patrimônio financiada por capital de terceiros." },
            { "Grau de Endividamento", "Mede o peso das dívidas em relação ao patrimônio líquido." },
            { "Composição do Endividamento", "Mostra quanto do endividamento total é de curto prazo." },
            { "Margem Operacional", "Revela o lucro obtido nas operações principais da empresa." },
            { "Margem Líquida", "Indica quanto do faturamento se transforma em lucro líquido." },
            { "Retorno sobre o Ativo (ROA)", "Mede a eficiência da empresa em gerar lucro com seus ativos." },
            { "Retorno sobre o Patrimônio Líquido (ROE)", "Avalia o retorno do investimento dos sócios." },
            { "Giro do Estoque", "Mostra quantas vezes o estoque é renovado em determinado período." },
            { "Prazo Médio de Pagamento (PMP)", "Indica o tempo médio que a empresa leva para pagar seus fornecedores." },
            { "Prazo Médio de Estocagem (PME)", "Mede o tempo médio que os produtos permanecem em estoque." },
            { "Prazo Médio de Recebimento (PMR)", "Mostra quanto tempo a empresa leva para receber de seus clientes." },
            { "Ciclo Financeiro", "Representa o tempo entre o pagamento aos fornecedores e o recebimento das vendas." },
            { "Necessidade de Capital de Giro (NCG)", "Mede quanto recurso a operação exige para financiar o ciclo operacional." },
            { "Capital de Giro de Longo Prazo (CGLP)", "Indica o volume de recursos permanentes que financiam o ativo circulante, demonstrando equilíbrio estrutural de longo prazo." }

        };

        public static string ObterDescricao(string nomeIndicador)
        {
            return _descricaoIndicadores.TryGetValue(nomeIndicador, out var descricao)
                ? descricao
                : "Descrição não disponível.";
        }
    }
}
