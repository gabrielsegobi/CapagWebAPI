using System.Text.Json.Serialization;

namespace Domain.Contracts.Empresas
{
    public class UpdateEmpresaRequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("razao_social")]
        public string RazaoSocial { get; set; } = string.Empty;
        [JsonPropertyName("nome_fantasia")]
        public string? NomeFantasia { get; set; } = string.Empty;
        [JsonPropertyName("matriz_filial")]
        public string MatrizFilial { get; set; } = string.Empty;
        [JsonPropertyName("id_empresa_matriz")]
        public long? IdEmpresaMatriz { get; set; }
        [JsonPropertyName("ativa")]
        public bool Ativa { get; set; }
        public string Cnae { get; set; } = string.Empty;
        [JsonPropertyName("municipio_estado")]
        public string MunicipioEstado { get; set; } = string.Empty;
        [JsonPropertyName("data_abertura")]
        public DateTime? DataAbertura { get; set; }
        [JsonPropertyName("capital_social")]
        public string CapitalSocial { get; set; } = string.Empty;
        [JsonPropertyName("segmento")]
        public string Segmento { get; set; } = string.Empty;
        [JsonPropertyName("porte")]
        public string Porte { get; set; } = string.Empty;
    }
}
