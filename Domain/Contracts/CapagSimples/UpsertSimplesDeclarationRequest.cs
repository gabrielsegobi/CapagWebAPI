using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagSimples
{
    public class UpsertSimplesDeclarationRequest
    {
        [JsonPropertyName("declarationKind")]
        public string DeclarationKind { get; set; } = string.Empty;

        [JsonPropertyName("exerciseYears")]
        public List<int>? ExerciseYears { get; set; }
    }
}
