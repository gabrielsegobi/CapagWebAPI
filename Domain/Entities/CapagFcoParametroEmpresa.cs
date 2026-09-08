namespace Domain.Entities
{
    /// <summary>
    /// Exceções FCO (L100/L300) por empresa. O catálogo universal fica no client;
    /// aqui só o delta em relação ao default.
    /// </summary>
    public class CapagFcoParametroEmpresa : ITenantEntity
    {
        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public long IdTenant { get; set; }
        public string ExcecoesL100Json { get; set; } = "{}";
        public string ExcecoesL300Json { get; set; } = "{}";
        public string? BaseVersaoHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? IdUsuario { get; set; }
    }
}
