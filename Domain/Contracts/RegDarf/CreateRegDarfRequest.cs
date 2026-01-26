using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDarf
{
    public class CreateRegDarfRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("requests")]
        public List<RegDarfItemRequest> Requests { get; set; } = new();
    }
    public class RegDarfItemRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("data_arrecadacao")]
        public DateOnly DataArrecadacao { get; set; }
        [JsonPropertyName("valor_total")]
        public decimal ValorTotal { get; set; }
    }
}
