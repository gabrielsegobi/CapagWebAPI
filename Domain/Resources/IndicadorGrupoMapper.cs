namespace Domain.Resources
{
    public static class IndicadorGrupoMapper
    {
        private static readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Liquidez Geral", "Liquidez" },
            { "Liquidez Seca", "Liquidez" },
            { "Liquidez Imediata", "Liquidez" },
            { "Capital de Giro de Longo Prazo (CGLP)", "Liquidez" },

            { "Índice de Endividamento Geral", "Endividamento" },
            { "Grau de Endividamento", "Endividamento" },
            { "Composição do Endividamento", "Endividamento" },


            { "Margem Operacional", "Rentabilidade e Eficiência" },
            { "Margem Líquida", "Rentabilidade e Eficiência" },
            { "Retorno sobre o Ativo (ROA)", "Rentabilidade e Eficiência" },
            { "Retorno sobre o Patrimônio Líquido (ROE)", "Rentabilidade e Eficiência" },

            { "Giro do Estoque", "Ciclo Financeiro" },
            { "Prazo Médio de Pagamento (PMP)", "Ciclo Financeiro" },
            { "Prazo Médio de Estocagem (PME)", "Ciclo Financeiro" },
            { "Prazo Médio de Recebimento (PMR)", "Ciclo Financeiro" },
            { "Ciclo Financeiro", "Ciclo Financeiro" },
            { "Necessidade de Capital de Giro (NCG)", "Ciclo Financeiro" }

        };

        private static readonly Dictionary<string, string> _descricaoGrupos = new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "Liquidez",
                "Avaliam a capacidade da empresa em honrar suas obrigações de curto prazo, mostrando se ela possui recursos suficientes para pagar suas dívidas imediatas sem comprometer o funcionamento das operações."
            },
            {
                "Endividamento",
                "Medem o grau de dependência da empresa em relação a capital de terceiros, indicando quanto do seu ativo é financiado por dívidas e o nível de risco financeiro assumido."
            },
            {
                "Rentabilidade e Eficiência",
                "Analisam o quanto a empresa é capaz de gerar lucro a partir de suas vendas, ativos e patrimônio líquido, revelando a eficiência na gestão dos recursos e a capacidade de gerar retorno aos investidores."
            },
            {
                "Ciclo Financeiro",
                "Mostra o tempo médio necessário para a empresa transformar investimentos em estoque e contas a receber em dinheiro novamente, sendo um importante indicador de gestão de caixa e capital de giro."
            }
        };

        public static string ObterGrupo(string nomeIndicador)
        {
            return _map.TryGetValue(nomeIndicador, out var grupo)
                ? grupo
                : "Não Classificado";
        }

        public static string ObterDescricaoGrupo(string nomeIndicador)
        {
            var grupo = ObterGrupo(nomeIndicador);
            return _descricaoGrupos.TryGetValue(grupo, out var descricao)
                ? descricao
                : "Descrição do grupo não disponível.";
        }
    }
}
