using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagFco
{
    /// <summary>
    /// Contrato camelCase estável para o client-app (fcoParametrosService).
    /// companyId = id_empresa da tabela empresas (Capag), não company_id do GMS.
    /// </summary>
    public class CapagFcoParametrosResponse
    {
        [JsonPropertyName("companyId")]
        public long CompanyId { get; set; }

        [JsonPropertyName("excecoesL100")]
        public JsonObject ExcecoesL100 { get; set; } = new();

        [JsonPropertyName("excecoesL300")]
        public JsonObject ExcecoesL300 { get; set; } = new();

        [JsonPropertyName("baseVersaoHash")]
        public string? BaseVersaoHash { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("contasPersonalizadasL100")]
        public int ContasPersonalizadasL100 { get; set; }

        [JsonPropertyName("contasPersonalizadasL300")]
        public int ContasPersonalizadasL300 { get; set; }
    }
}
