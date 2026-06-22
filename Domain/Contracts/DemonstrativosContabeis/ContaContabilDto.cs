using System.Text.Json.Serialization;

namespace Domain.Contracts.DemonstrativosContabeis
{
    public class ContaContabilDto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;
    }
}
