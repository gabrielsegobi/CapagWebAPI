using System.Text.Json.Serialization;

namespace Domain.Contracts.TipoGrupos
{
    public class GetVariaveisRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("ano")]
        public string Ano { get; set; } = string.Empty;
    }
}
