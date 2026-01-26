using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsPgdasd
{
    public class CreateRegPgdasdRequest
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("requests")]
        public List<RegPgdasdItemRequest> Requests { get; set; } = new();
    }

    public class RegPgdasdItemRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("periodo")]
        public string Periodo { get; set; } = string.Empty;
        [JsonPropertyName("receita_bruta")]
        public decimal ReceitaBruta { get; set; }
        [JsonPropertyName("total_debito")]
        public decimal TotalDebito { get; set; }
    }
}
