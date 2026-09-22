using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ContaPrlAAjusteMap : IEntityTypeConfiguration<ContaPrlAAjuste>
    {
        public void Configure(EntityTypeBuilder<ContaPrlAAjuste> builder)
        {
            builder.ToTable("conta_prla_ajuste");
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

            builder.Property(e => e.Acao)
                .HasColumnName("acao")
                .HasColumnType("VARCHAR(32)");

            builder.Property(e => e.Justificativa)
                .HasColumnName("justificativa")
                .HasColumnType("VARCHAR(500)");

            builder.Property(e => e.SaldoManual)
                .HasColumnName("saldo_manual")
                .HasColumnType("DECIMAL(20,2)");

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
