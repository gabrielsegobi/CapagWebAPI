using System.Text.Json.Serialization;
using Domain.Enums;

namespace Domain.Contracts.PrlA
{
    public class ContaPrlADto
    {
        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("bloco_original")]
        public BlocoLiquidez BlocoOriginal { get; set; }

        [JsonPropertyName("bloco_ajustado")]
        public BlocoLiquidez? BlocoAjustado { get; set; }

        [JsonPropertyName("bloco_efetivo")]
        public BlocoLiquidez BlocoEfetivo { get; set; }

        [JsonPropertyName("percentual_desagio")]
        public decimal PercentualDesagio { get; set; }

        [JsonPropertyName("desagio_manual")]
        public bool DesagioManual { get; set; }

        [JsonPropertyName("saldo_original")]
        public decimal SaldoOriginal { get; set; }

        [JsonPropertyName("saldo_ajustado")]
        public decimal? SaldoAjustado { get; set; }

        [JsonPropertyName("saldo_normalizado")]
        public decimal SaldoNormalizado { get; set; }
    }
}
