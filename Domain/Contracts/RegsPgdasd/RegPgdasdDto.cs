using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsPgdasd
{
    public class RegPgdasdDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("periodo")]
        public string Periodo { get; set; } = string.Empty;
        [JsonPropertyName("receita_bruta")]
        public decimal ReceitaBruta { get; set; }
        [JsonPropertyName("total_debito")]
        public decimal TotalDebito { get; set; }
        [JsonPropertyName("id_filename")]
        public long IdFilename { get; set; }
    }
}
