namespace Domain.Contracts.Json
{
    public class FormulaJson
    {
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Identificador curto estável para composição de fórmulas (<c>{@PME}</c>).
        /// Obrigatório apenas para indicadores referenciados por outras fórmulas.
        /// </summary>
        public string? Tag { get; set; }

        public string Formula { get; set; } = string.Empty;
        public string Desc_Formula { get; set; } = string.Empty;
    }
}
