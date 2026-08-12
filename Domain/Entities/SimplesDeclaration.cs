namespace Domain.Entities
{
    public class SimplesDeclaration : ITenantEntity
    {
        public long Id { get; set; }
        public long IdEmpresa { get; set; }
        public long IdTenant { get; set; }
        public long? IdUsuario { get; set; }
        public string DeclarationKind { get; set; } = string.Empty;
        public DateTime DateCreate { get; set; }
        public DateTime DateUpdate { get; set; }

        public ICollection<SimplesExerciseYear> ExerciseYears { get; set; } = new List<SimplesExerciseYear>();
    }
}
