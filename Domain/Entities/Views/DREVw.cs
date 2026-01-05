namespace Domain.Entities.Views
{
    public class DREVw
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime? DtIni { get; set; }
        public DateTime? DtIniApur { get; set; }
        public DateTime? DtFinApur { get; set; }
        public string? PerApur { get; set; }
        public int Ano { get; set; }
        public string? Codigo { get; set; }
        public string? Descricao { get; set; }
        public char? Tipo { get; set; }
        public byte? Nivel { get; set; }
        public decimal? ValCtaRefIni { get; set; }
        public char? IndValCtaRefIni { get; set; }
        public decimal? ValCtaRefDeb { get; set; }
        public decimal? ValCtaRefCred { get; set; }
        public decimal? ValCtaRefFin { get; set; }
        public char? IndValCtaRefFin { get; set; }
        public string? TipoTrib { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
