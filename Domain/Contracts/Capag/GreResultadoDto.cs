using System.Text.Json.Serialization;
using Domain.Contracts.Demonstrativos;

namespace Domain.Contracts.Capag
{
    public class GreResultadoDto
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("anos")]
        public List<int> Anos { get; set; } = new();

        [JsonPropertyName("linhas")]
        public List<LinhaDemonstrativoDto> Linhas { get; set; } = new();

        [JsonPropertyName("lucro_bruto")]
        public LinhaDemonstrativoDto? LucroBruto { get; set; }
    }
}
