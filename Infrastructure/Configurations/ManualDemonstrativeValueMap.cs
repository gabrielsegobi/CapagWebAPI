using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ManualDemonstrativeValueMap : IEntityTypeConfiguration<ManualDemonstrativeValue>
    {
        public void Configure(EntityTypeBuilder<ManualDemonstrativeValue> builder)
        {
            builder.ToTable("manual_demonstrative_value");
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

            builder.Property(e => e.DemonstrativeKind)
                .HasColumnName("demonstrative_kind")
                .HasColumnType("ENUM('DRE','BALANCE_SHEET')")
                .IsRequired();

            builder.Property(e => e.AccountCode)
                .HasColumnName("account_code")
                .HasColumnType("VARCHAR(64)")
                .IsRequired();

            builder.Property(e => e.ExerciseYear)
                .HasColumnName("exercise_year")
                .HasColumnType("SMALLINT")
                .IsRequired();

            builder.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasColumnType("DECIMAL(20,4)")
                .IsRequired();

            builder.Property(e => e.DateCreate)
                .HasColumnName("date_create")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.DateUpdate)
                .HasColumnName("date_update")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.HasIndex(e => new
            {
                e.IdEmpresa,
                e.DemonstrativeKind,
                e.AccountCode,
                e.ExerciseYear
            }).IsUnique();
        }
    }
}
