using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ContaGreManualMap : IEntityTypeConfiguration<ContaGreManual>
    {
        public void Configure(EntityTypeBuilder<ContaGreManual> builder)
        {
            builder.ToTable("conta_gre_manual");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.EmpresaId)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.CodigoConta)
                .HasColumnName("codigo_conta")
                .HasColumnType("VARCHAR(64)")
                .IsRequired();

            builder.Property(e => e.CodigoPai)
                .HasColumnName("codigo_pai")
                .HasColumnType("VARCHAR(64)");

            builder.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasColumnType("VARCHAR(255)")
                .IsRequired();

            builder.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasColumnType("VARCHAR(16)")
                .IsRequired();

            builder.Property(e => e.UsarMedia)
                .HasColumnName("usar_media")
                .HasColumnType("TINYINT(1)")
                .IsRequired();

            builder.Property(e => e.Justificativa)
                .HasColumnName("justificativa")
                .HasColumnType("VARCHAR(500)");

            builder.Property(e => e.ValoresJson)
                .HasColumnName("valores_json")
                .HasColumnType("json")
                .IsRequired();

            builder.Property(e => e.AtualizadoEm)
                .HasColumnName("atualizado_em")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.AtualizadoPor)
                .HasColumnName("atualizado_por")
                .HasColumnType("VARCHAR(128)")
                .IsRequired();

            builder.HasIndex(e => new { e.EmpresaId, e.CodigoConta }).IsUnique();
        }
    }
}
