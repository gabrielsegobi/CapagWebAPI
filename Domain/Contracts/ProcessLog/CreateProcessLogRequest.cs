using System.Text.Json.Serialization;

namespace Domain.Contracts.ProcessLog
{
    public class CreateProcessLogRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("acao")]
        public string Acao { get; set; } = string.Empty;
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        //public long IdTenant { get; set; }
    }
}
