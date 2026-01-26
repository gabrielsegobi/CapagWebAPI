using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDefis
{
    public class CreateRegDefisRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("requests")]
        public List<CreateRegDefisItemRequest> Requests { get; set; } = new();
    }

    public class CreateRegDefisItemRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("periodo")]
        public DateTime Periodo { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
