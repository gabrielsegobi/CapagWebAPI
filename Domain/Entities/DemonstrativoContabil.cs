namespace Domain.Entities
{
    public class DemonstrativoContabil
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateOnly DtIni { get; set; }
        public DateOnly DtIniApur { get; set; }
        public DateOnly DtFinApur { get; set; }
        public string PerApur { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public char Tipo { get; set; }
        public byte Nivel { get; set; }
        public decimal? ValCtaRefIni { get; set; }
        public char? IndValCtaRefIni { get; set; }
        public decimal? ValCtaRefDeb { get; set; }
        public decimal? ValCtaRefCred { get; set; }
        public decimal? ValCtaRefFin { get; set; }
        public char? IndValCtaRefFin { get; set; }
        public string TipoTrib { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
