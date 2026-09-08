namespace Domain.Entities
{
    /// <summary>
    /// Snapshot do cálculo CAPAG-e1 (GRE/PLRA/ajustes/resultado).
    /// O documento completo fica em <see cref="PayloadJson"/> (JSON opaco).
    /// </summary>
    public class CapagE1Calculo : ITenantEntity
    {
        public long Id { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public string Modelo { get; set; } = "capag-e-1";
        public string PayloadJson { get; set; } = "{}";
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
        public long? IdUsuario { get; set; }
    }
}
