using System.Text.Json.Serialization;

namespace Domain.Contracts.DemonstrativosContabeis
{
    public class CreateDRERequest
    {
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }

        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }

        [JsonPropertyName("dt_ini")]
        public DateOnly DtIni { get; set; }

        [JsonPropertyName("dt_ini_apur")]
        public DateOnly DtIniApur { get; set; }

        [JsonPropertyName("dt_fin_apur")]
        public DateOnly DtFinApur { get; set; }

        [JsonPropertyName("per_apur")]
        public string PerApur { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonPropertyName("tipo")]
        public char Tipo { get; set; }

        [JsonPropertyName("nivel")]
        public byte Nivel { get; set; }

        [JsonPropertyName("val_cta_ref_fin")]
        public decimal? ValCtaRefFin { get; set; }

        [JsonPropertyName("ind_val_cta_ref_fin")]
        public char? IndValCtaRefFin { get; set; }

        [JsonPropertyName("tipo_trib")]
        public string TipoTrib { get; set; } = string.Empty;
    }
}
