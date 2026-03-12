using System.Text.Json.Serialization;

namespace Domain.Contracts.RegsDctf
{
    public class RegDctfDto
    {
        //[JsonPropertyName("id")]
        //public long Id { get; set; }
        //[JsonPropertyName("id_empresa")]
        //public long IdEmpresa { get; set; }
        //[JsonPropertyName("id_tenant")]
        //public long IdTenant { get; set; }
        //[JsonPropertyName("id_filename")]
        //public long IdFilename { get; set; }
        [JsonPropertyName("periodo")]
        public string Periodo { get; set; } = string.Empty;
        [JsonPropertyName("recibo_retificadora")]
        public string ReciboRetificadora { get; set; } = string.Empty;

        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
    }
}
