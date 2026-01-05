namespace Domain.Entities
{
    public class IndicadoresLiquidez
    {
        public long IdIndicador { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string SaudeParametro { get; set; } = string.Empty;
        public decimal SaudeEmpresa { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
