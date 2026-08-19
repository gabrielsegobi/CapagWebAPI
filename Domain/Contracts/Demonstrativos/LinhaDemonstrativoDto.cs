using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Contracts.Demonstrativos
{
    public class LinhaDemonstrativoDto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("nivel")]
        public byte Nivel { get; set; }

        [JsonPropertyName("posicao")]
        public int Posicao { get; set; }

        [JsonPropertyName("grupo")]
        public GrupoContabil Grupo { get; set; }

        [JsonPropertyName("indicador_dc")]
        public IndicadorDC? IndicadorDC { get; set; }

        [JsonPropertyName("is_linha_calculada")]
        public bool IsLinhaCalculada { get; set; }

        [JsonPropertyName("excluida")]
        public bool Excluida { get; set; }

        [JsonPropertyName("valores")]
        public ValoresAnuaisDto Valores { get; set; } = new();

        [JsonPropertyName("total")]
        public decimal Total => Valores.Total;

        [JsonPropertyName("filhos")]
        public List<LinhaDemonstrativoDto> Filhos { get; set; } = new();
    }
}
