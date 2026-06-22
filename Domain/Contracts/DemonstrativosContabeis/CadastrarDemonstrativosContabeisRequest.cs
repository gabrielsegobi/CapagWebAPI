using System.Text.Json.Serialization;

namespace Domain.Contracts.DemonstrativosContabeis
{
    public class CadastrarDemonstrativosContabeisRequest
    {
        [JsonPropertyName("sobrescrever")]
        public bool Sobrescrever { get; set; }

        [JsonPropertyName("contas")]
        public List<CadastrarDemonstrativosContabeisItem> Contas { get; set; } = new();
    }
}
