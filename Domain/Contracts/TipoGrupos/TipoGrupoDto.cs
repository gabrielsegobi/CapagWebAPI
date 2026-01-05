using System.Text.Json.Serialization;

namespace Domain.Contracts.TipoGrupos
{
    public class TipoGrupoDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("tag")]
        public string Tag { get; set; } = string.Empty;
    }
}
