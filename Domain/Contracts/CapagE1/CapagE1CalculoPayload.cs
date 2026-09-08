using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagE1
{
    /// <summary>
    /// Documento CapagE1CalculoApiPayload (snake_case).
    /// Nested JSON (gre_linhas etc.) usa JsonNode para não strippar
    /// campos como valoresSinalInvertido.
    /// </summary>
    public class CapagE1CalculoPayload
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("modelo")]
        public string Modelo { get; set; } = "capag-e-1";

        [JsonPropertyName("anos")]
        public JsonNode? Anos { get; set; }

        [JsonPropertyName("parametros")]
        public JsonNode? Parametros { get; set; }

        [JsonPropertyName("gre_linhas")]
        public JsonNode? GreLinhas { get; set; }

        [JsonPropertyName("plra_linhas")]
        public JsonNode? PlraLinhas { get; set; }

        [JsonPropertyName("contas_manuais_gre")]
        public JsonNode? ContasManuaisGre { get; set; }

        [JsonPropertyName("valor_divida")]
        public decimal? ValorDivida { get; set; }

        [JsonPropertyName("resultado")]
        public JsonNode? Resultado { get; set; }

        [JsonPropertyName("atualizado_em")]
        public DateTime? AtualizadoEm { get; set; }
    }
}
