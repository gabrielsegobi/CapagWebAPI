using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CapagE1CalculoMap : IEntityTypeConfiguration<CapagE1Calculo>
    {
        public void Configure(EntityTypeBuilder<CapagE1Calculo> builder)
        {
            builder.ToTable("capag_e1_calculo");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT UNSIGNED")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(e => e.IdEmpresa)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(e => e.Modelo)
                .HasColumnName("modelo")
                .HasColumnType("VARCHAR(32)")
                .IsRequired();

            builder.Property(e => e.PayloadJson)
                .HasColumnName("payload_json")
                .HasColumnType("json")
                .IsRequired();

            builder.Property(e => e.DateCreate)
                .HasColumnName("date_create")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.DateUpdate)
                .HasColumnName("date_update")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario")
                .HasColumnType("BIGINT");

            builder.HasIndex(e => new { e.IdEmpresa, e.Modelo }).IsUnique();
        }
    }
}
