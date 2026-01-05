using Domain.Contracts.ValoresAnuais;
using Domain.Entities;
using Domain.Resources;
using System.Text.Json.Serialization;

namespace Domain.Contracts.Indicadores
{
    public class IndicadorDto
    {
        [JsonPropertyName("id_indicador")]
        public long IdIndicador { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("saude_empresa")]
        public decimal? SaudeEmpresa { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("grupo")]
        public string Grupo { get; set; } = string.Empty;

        [JsonPropertyName("grupo_descricao")]
        public string GrupoDescricao { get; set; } = string.Empty;

        [JsonPropertyName("descricao_indicador")]
        public string DescricaoIndicador { get; set; } = string.Empty;


        [JsonPropertyName("valores_anuais")]
        public ICollection<ValorAnualDto> ValoresAnuais { get; set; } = new List<ValorAnualDto>();
    }
}
