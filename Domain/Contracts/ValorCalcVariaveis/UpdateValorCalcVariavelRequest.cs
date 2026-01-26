using System.Text.Json.Serialization;

namespace Domain.Contracts.ValorCalcVariaveis
{
    public class UpdateValorCalcVariavelRequest
    {
        [JsonPropertyName("id_tipo_grupo")]
        public long IdTipoGrupo { get; set; }
        [JsonPropertyName("ano_base")]
        public short AnoBase { get; set; }
        [JsonPropertyName("id_variavel")]
        public string IdVariavel { get; set; } = string.Empty;
        [JsonPropertyName("valor")]
        public decimal Valor { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
