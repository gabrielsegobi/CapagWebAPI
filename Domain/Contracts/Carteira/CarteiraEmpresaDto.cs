using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class CarteiraEmpresaDto
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("nome_empresa")]
        public string NomeEmpresa { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("valor_contrato")]
        public decimal? ValorContrato { get; set; }

        [JsonPropertyName("data_impedimento")]
        public DateTime? DataImpedimento { get; set; }

        [JsonPropertyName("status_bloqueio")]
        public string StatusBloqueio { get; set; } = string.Empty;

        [JsonPropertyName("rating_capag")]
        public string? RatingCapag { get; set; }

        [JsonPropertyName("ultimo_ano_ecf")]
        public int? UltimoAnoEcf { get; set; }

        [JsonPropertyName("data_calculo")]
        public DateTime? DataCalculo { get; set; }

        [JsonPropertyName("responsavel")]
        public string? Responsavel { get; set; }
    }
}
