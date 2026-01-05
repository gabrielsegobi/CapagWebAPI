namespace Domain.Entities
{
    public class AnaliseICP
    {
        public long IdAnalise { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public decimal IcpCalculado { get; set; }
        public string? Classificacao { get; set; } = string.Empty;
        public decimal SomaPesos { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
