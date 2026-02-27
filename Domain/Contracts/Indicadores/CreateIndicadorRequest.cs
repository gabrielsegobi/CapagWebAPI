using System.Text.Json.Serialization;

namespace Domain.Contracts.Indicadores
{
    public class CreateIndicadorRequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("saude_empresa")]
        public decimal? SaudeEmpresa { get; set; }

        [JsonPropertyName("valores_calc_saude_empresa")]
        public string ValoresCalcSaudeEmpresa { get; set; } = string.Empty;
    }
}
