namespace Domain.Entities
{
    public class ManualDemonstrativeValue : ITenantEntity
    {
        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public long IdTenant { get; set; }
        public long? IdUsuario { get; set; }
        public string DemonstrativeKind { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public int ExerciseYear { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }
    }
}
