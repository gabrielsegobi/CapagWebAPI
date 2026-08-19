using System.Text.Json.Serialization;

namespace Domain.Contracts.Carteira
{
    public class PainelContratosDto
    {
        [JsonPropertyName("empresas_com_calculo_efetuado")]
        public int EmpresasComCalculoEfetuado { get; set; }

        [JsonPropertyName("em_negociacao")]
        public int EmNegociacao { get; set; }

        [JsonPropertyName("valor_estimado_das_negociacoes")]
        public decimal ValorEstimadoDasNegociacoes { get; set; }

        [JsonPropertyName("contratos_fechados")]
        public int ContratosFechados { get; set; }

        [JsonPropertyName("valor_arrecadado_contratos_fechados")]
        public decimal ValorArrecadadoContratosFechados { get; set; }
    }
}
