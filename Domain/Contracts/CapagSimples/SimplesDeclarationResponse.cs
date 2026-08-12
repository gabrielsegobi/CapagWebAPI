using System.Text.Json.Serialization;

namespace Domain.Contracts.CapagSimples
{
    /// <summary>
    /// Contrato camelCase estável para o client-app (gmaster-system).
    /// </summary>
    public class SimplesDeclarationResponse
    {
        [JsonPropertyName("hasDeclaration")]
        public bool HasDeclaration { get; set; }

        [JsonPropertyName("declarationKind")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? DeclarationKind { get; set; }

        [JsonPropertyName("exerciseYears")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<int>? ExerciseYears { get; set; }

        [JsonPropertyName("createdAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdatedAt { get; set; }

        public static SimplesDeclarationResponse Empty() => new() { HasDeclaration = false };
    }
}
