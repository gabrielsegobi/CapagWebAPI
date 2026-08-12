using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SimplesDeclarationMap : IEntityTypeConfiguration<SimplesDeclaration>
    {
        public void Configure(EntityTypeBuilder<SimplesDeclaration> builder)
        {
            builder.ToTable("simples_declaration");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdEmpresa)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario")
                .HasColumnType("BIGINT");

            builder.Property(e => e.DeclarationKind)
                .HasColumnName("declaration_kind")
                .HasColumnType("ENUM('SIMPLES_EXERCISE_YEARS','NO_NATIONAL_SIMPLE_STRICT')")
                .IsRequired();

            builder.Property(e => e.DateCreate)
                .HasColumnName("date_create")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.DateUpdate)
                .HasColumnName("date_update")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.HasIndex(e => e.IdEmpresa).IsUnique();

            builder.HasMany(e => e.ExerciseYears)
                .WithOne(y => y.Declaration)
                .HasForeignKey(y => y.IdSimplesDeclaration)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
