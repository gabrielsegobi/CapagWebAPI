using System.Text.Json.Serialization;

namespace Domain.Contracts.RegDefis
{
    public class RegDefisDto
    {

        //[JsonPropertyName("id")]
        //public long Id { get; set; }
        //[JsonPropertyName("id_empresa")]
        //public long IdEmpresa { get; set; }
        //[JsonPropertyName("id_tenant")]
        //public long IdTenant { get; set; }
        [JsonPropertyName("periodo")]
        public DateTime Periodo { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
        //[JsonPropertyName("id_filename")]
        //public long IdFilename { get; set; }
    }
}
