namespace Domain.Entities
{
    public class RegimeTributario
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public DateOnly? DtIni { get; set; }
        public int Ano { get; set; }
        public string RaizCnpj { get; set; } = string.Empty;
        public string? FormaTribCompleta { get; set; } = string.Empty;
        public string? FormaApurCompleta { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
