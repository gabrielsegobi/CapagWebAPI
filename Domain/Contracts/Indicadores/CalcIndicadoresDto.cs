using Domain.Contracts.ValoresAnuais;
using System.Text.Json.Serialization;

namespace Domain.Contracts.Indicadores
{
    public class CalcIndicadoresDto
    {
        [JsonPropertyName("id_indicador")]
        public long IdIndicador { get; set; }
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;
        [JsonPropertyName("saude_empresa")]
        public decimal SaudeEmpresa { get; set; }
        [JsonPropertyName("valores_calc_saude_empresa")]
        public string ValoresCalcSaudeEmpresa { get; set; } = string.Empty;

        public List<CalcValoresAnuaisDto> ValoresAnuais { get; set; } = [];
    }
}
