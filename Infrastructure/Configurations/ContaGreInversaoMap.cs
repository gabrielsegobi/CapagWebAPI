using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ContaGreInversaoMap : IEntityTypeConfiguration<ContaGreInversao>
    {
        public void Configure(EntityTypeBuilder<ContaGreInversao> builder)
        {
            builder.ToTable("conta_gre_inversao");
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

            builder.Property(e => e.Ano)
                .HasColumnName("ano")
                .HasColumnType("SMALLINT")
                .IsRequired();

            builder.Property(e => e.Invertido)
                .HasColumnName("invertido")
                .HasColumnType("TINYINT(1)")
                .IsRequired();

            builder.Property(e => e.Justificativa)
                .HasColumnName("justificativa")
                .HasColumnType("VARCHAR(500)");

            builder.Property(e => e.AtualizadoEm)
                .HasColumnName("atualizado_em")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.AtualizadoPor)
                .HasColumnName("atualizado_por")
                .HasColumnType("VARCHAR(128)")
                .IsRequired();

            builder.HasIndex(e => new { e.EmpresaId, e.CodigoConta, e.Ano }).IsUnique();
        }
    }
}
