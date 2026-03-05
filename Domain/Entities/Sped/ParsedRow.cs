namespace Domain.Entities.Sped
{
    /// <summary>
    /// Struct que trafega pelo Channel entre o parser e o BulkWriter.
    /// Responsabilidade: transportar o nome da tabela de destino e o buffer de valores.
    /// É o contrato entre parser e writer — não tem comportamento.
    /// Struct (não class) para evitar alocação no heap por linha.
    /// </summary>
    public sealed class ParsedRow
    {
        /// <summary>
        /// Nome da tabela de destino no banco. Ex: "SpedEcfP100"
        /// </summary>
        public string Tabela { get; }

        /// <summary>
        /// Buffer com todos os valores da linha já convertidos para os tipos corretos.
        /// </summary>
        public ValueBuffer Buffer { get; }

        public ParsedRow(string tabela, ValueBuffer buffer)
        {
            Tabela = tabela;
            Buffer = buffer;
        }
    }
}
