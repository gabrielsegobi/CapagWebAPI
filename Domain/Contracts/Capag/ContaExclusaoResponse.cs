using System.Text.Json.Serialization;
using Domain.Contracts.Demonstrativos;

namespace Domain.Contracts.Capag
{
    public class ContaExclusaoResponse
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("codigo_conta")]
        public string CodigoConta { get; set; } = string.Empty;

        [JsonPropertyName("excluida")]
        public bool Excluida { get; set; }

        [JsonPropertyName("justificativa")]
        public string? Justificativa { get; set; }

        [JsonPropertyName("atualizado_em")]
        public DateTime AtualizadoEm { get; set; }

        [JsonPropertyName("atualizado_por")]
        public string AtualizadoPor { get; set; } = string.Empty;

        [JsonPropertyName("gre")]
        public GreResultadoDto? Gre { get; set; }
    }
}
