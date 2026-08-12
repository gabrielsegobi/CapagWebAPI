using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SimplesExerciseYearMap : IEntityTypeConfiguration<SimplesExerciseYear>
    {
        public void Configure(EntityTypeBuilder<SimplesExerciseYear> builder)
        {
            builder.ToTable("simples_exercise_year");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdSimplesDeclaration)
                .HasColumnName("id_simples_declaration")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.ExerciseYear)
                .HasColumnName("exercise_year")
                .HasColumnType("SMALLINT")
                .IsRequired();

            builder.Property(e => e.DateCreate)
                .HasColumnName("date_create")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.HasIndex(e => new { e.IdSimplesDeclaration, e.ExerciseYear }).IsUnique();
        }
    }
}
