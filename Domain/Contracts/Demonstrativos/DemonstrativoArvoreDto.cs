using System.Text.Json.Serialization;

namespace Domain.Contracts.Demonstrativos
{
    public class DemonstrativoArvoreDto
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("anos")]
        public List<int> Anos { get; set; } = new();

        [JsonPropertyName("linhas")]
        public List<LinhaDemonstrativoDto> Linhas { get; set; } = new();

        [JsonPropertyName("resultado_liquido")]
        public ValoresAnuaisDto? ResultadoLiquido { get; set; }
    }
}
