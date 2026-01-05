using Domain.Contracts.ResultadosIndicesICP;
using Domain.Entities;
using System.Text.Json.Serialization;

namespace Domain.Contracts.ModelosIndicesICP
{
    public class ModeloIndiceICPDto
    {
        [JsonPropertyName("id_modelo_indice")]
        public long IdModeloIndice { get; set; }
        [JsonPropertyName("id_tenant")]
        public long? IdTenant { get; set; }
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("formula")]
        public string Formula { get; set; } = string.Empty;
        [JsonPropertyName("meta")]
        public decimal Meta { get; set; }
        [JsonPropertyName("pior_caso")]
        public decimal PiorCaso { get; set; }
        [JsonPropertyName("peso")]
        public decimal Peso { get; set; }
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("resultados")]
        public ICollection<ResultadoIndiceICPDto> Resultados { get; set; } = new List<ResultadoIndiceICPDto>();
    }
}
