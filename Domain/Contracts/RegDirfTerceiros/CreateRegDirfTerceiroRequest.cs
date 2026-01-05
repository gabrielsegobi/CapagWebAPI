using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("request")]
        public List<RegDirfTerceiroItemRequest> Requests { get; set; } = new();
    }
    public class RegDirfTerceiroItemRequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;
        [JsonPropertyName("valor_rendimento")]
        public decimal ValorRendimento { get; set; }
        [JsonPropertyName("valor_tributo")]
        public decimal ValorTributo { get; set; }
        [JsonPropertyName("ano_calendario")]
        public string AnoCalendario { get; set; } = string.Empty;
        [JsonPropertyName("data_processamento")]
        public DateTime DataProcessamento { get; set; }

    }
}
