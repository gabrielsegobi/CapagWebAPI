using System.Text.Json.Serialization;

namespace Domain.Contracts.DemonstrativosContabeis
{
    public class ConstruirDemonstrativosRequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("anos")]
        public List<int> Anos { get; set; } = new();

        [JsonPropertyName("sobrescrever")]
        public bool Sobrescrever { get; set; }
    }
}

