using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsDctf
{
    public class CreateRegDctfRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("requests")]
        public List<RegDctfItemRequest> Requests { get; set; } = new();
    }

    public class RegDctfItemRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }

        [JsonPropertyName("periodo")]
        public string Periodo { get; set; } = string.Empty;
    }
}
