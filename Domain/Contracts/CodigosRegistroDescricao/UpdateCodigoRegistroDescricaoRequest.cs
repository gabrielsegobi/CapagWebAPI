using System.Text.Json.Serialization;

namespace Domain.Contracts.CodigosRegistroDescricao
{
    public class UpdateCodigoRegistroDescricaoRequest
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("expressao_regular")]
        public string ExpressaoRegular { get; set; } = string.Empty;
    }
}
