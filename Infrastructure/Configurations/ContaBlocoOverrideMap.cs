using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ContaBlocoOverrideMap : IEntityTypeConfiguration<ContaBlocoOverride>
    {
        public void Configure(EntityTypeBuilder<ContaBlocoOverride> builder)
        {
            builder.ToTable("conta_bloco_override");
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

            builder.Property(e => e.BlocoOriginal)
                .HasColumnName("bloco_original")
                .HasColumnType("CHAR(1)")
                .HasConversion(
                    v => v.ToString(),
                    v => Enum.Parse<BlocoLiquidez>(v))
                .IsRequired();

            builder.Property(e => e.BlocoAjustado)
                .HasColumnName("bloco_ajustado")
                .HasColumnType("CHAR(1)")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToString() : null,
                    v => string.IsNullOrEmpty(v) ? null : Enum.Parse<BlocoLiquidez>(v));

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
