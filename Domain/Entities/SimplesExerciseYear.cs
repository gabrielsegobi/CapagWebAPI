namespace Domain.Entities
{
    public class SimplesExerciseYear
    {
        public long Id { get; set; }
        public long IdSimplesDeclaration { get; set; }
        public int ExerciseYear { get; set; }
        public DateTime DateCreate { get; set; }

        public SimplesDeclaration? Declaration { get; set; }
    }
}
