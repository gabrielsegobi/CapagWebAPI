using System.Text.Json.Serialization;

namespace Domain.Contracts.Views
{
    public class DREViewDto
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("id_tenant")]
        public long IdTenant { get; set; }
        [JsonPropertyName("id_empresa")]
        public long IdEmpresa { get; set; }
        [JsonPropertyName("dt_ini")]
        public DateTime? DtIni { get; set; }
        [JsonPropertyName("dt_ini_apur")]
        public DateTime? DtIniApur { get; set; }
        [JsonPropertyName("dt_fin_apur")]
        public DateTime? DtFinApur { get; set; }
        [JsonPropertyName("per_apur")]
        public string? PerApur { get; set; }
        [JsonPropertyName("ano")]
        public int Ano { get; set; }
        [JsonPropertyName("codigo")]
        public string? Codigo { get; set; }
        [JsonPropertyName("descricao")]
        public string? Descricao { get; set; }
        [JsonPropertyName("tipo")]
        public char? Tipo { get; set; }
        [JsonPropertyName("nivel")]
        public byte? Nivel { get; set; }
        [JsonPropertyName("val_cta_ref_ini")]
        public decimal? ValCtaRefIni { get; set; }
        [JsonPropertyName("ind_val_cta_ref_ini")]
        public char? IndValCtaRefIni { get; set; }
        [JsonPropertyName("val_cta_ref_deb")]
        public decimal? ValCtaRefDeb { get; set; }
        [JsonPropertyName("val_cta_ref_cred")]
        public decimal? ValCtaRefCred { get; set; }
        [JsonPropertyName("val_cta_ref_fin")]
        public decimal? ValCtaRefFin { get; set; }
        [JsonPropertyName("ind_val_cta_ref_fin")]
        public char? IndValCtaRefFin { get; set; }
        [JsonPropertyName("tipo_trib")]
        public string? TipoTrib { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
