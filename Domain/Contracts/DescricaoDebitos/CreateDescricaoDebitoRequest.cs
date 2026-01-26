using System.Text.Json.Serialization;

namespace Domain.Contracts.DescricaoDebitos
{
    public class CreateDescricaoDebitoRequest
    {
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("natureza")]
        public string Natureza { get; set; } = string.Empty;

        [JsonPropertyName("num_cda")]
        public string NumCda { get; set; } = string.Empty;

        [JsonPropertyName("data_inscricao")]
        public DateTime DataInscricao { get; set; }

        [JsonPropertyName("valor_principal")]
        public decimal ValorPrincipal { get; set; }

        [JsonPropertyName("valor_multa")]
        public decimal ValorMulta { get; set; }

        [JsonPropertyName("valor_juros")]
        public decimal ValorJuros { get; set; }

        [JsonPropertyName("valor_encargos")]
        public decimal ValorEncargos { get; set; }
    }
}
