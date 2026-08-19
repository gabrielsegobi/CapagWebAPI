using Domain.Enums;

namespace Domain.Entities
{
    public class ContaBlocoOverride : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long EmpresaId { get; set; }
        public string CodigoConta { get; set; } = string.Empty;
        public BlocoLiquidez BlocoOriginal { get; set; }
        public BlocoLiquidez? BlocoAjustado { get; set; }
        public DateTime AtualizadoEm { get; set; }
        public string AtualizadoPor { get; set; } = string.Empty;
    }
}
