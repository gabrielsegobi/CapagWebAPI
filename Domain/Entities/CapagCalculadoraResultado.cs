namespace Domain.Entities
{
    public class CapagCalculadoraResultado : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Classificacao { get; set; } = string.Empty;
        public string PercentualExibicao { get; set; } = string.Empty;
        public string LabelMetrica { get; set; } = string.Empty;
        public string StatusMensagem { get; set; } = string.Empty;
        public decimal ValorCapag { get; set; }
        public decimal? ValorDivida { get; set; }
        public decimal? Indice { get; set; }
        public bool Parcial { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public long? IdUsuario { get; set; }
    }
}
