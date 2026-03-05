namespace Domain.Entities.Sped
{
    /// <summary>
    /// Representa um único campo dentro de um registro SPED.
    /// Responsabilidade: carregar metadados de um campo (nome, posição, tipo).
    /// Não faz nada além de transportar dados.
    /// </summary>
    public class FieldLayout
    {
        /// <summary>
        /// Nome da coluna no banco de dados. Ex: "Codigo", "Descricao"
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Índice do campo na linha separada por pipe.
        /// Ex: |0|P100|CODIGO|DESCRICAO| → Codigo está no índice 2
        /// </summary>
        public int Indice { get; set; }

        /// <summary>
        /// Tipo de dado para conversão: "string", "decimal", "int", "long", "date"
        /// </summary>
        public string Tipo { get; set; } = "string";

        /// <summary>
        /// Nome da coluna no banco.
        /// </summary>
        public string Coluna { get; set; } = string.Empty;
    }
}
