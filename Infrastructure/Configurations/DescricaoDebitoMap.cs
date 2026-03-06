using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DescricaoDebitoMap : IEntityTypeConfiguration<DescricaoDebito>
    {
        public void Configure(EntityTypeBuilder<DescricaoDebito> builder)
        {
            builder.ToTable("descricao_debitos");
            builder.HasKey(d => d.IdDescricaoDebitos);

            builder.Property(d => d.IdDescricaoDebitos).HasColumnName("id_descricao_debitos").HasColumnType("BIGINT").UseMySqlIdentityColumn();
            builder.Property(d => d.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(d => d.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(d => d.Natureza).HasColumnName("natureza").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(d => d.NumCda).HasColumnName("num_cda").HasColumnType("VARCHAR(50)");
            builder.Property(d => d.DataInscricao).HasColumnName("data_inscricao").HasColumnType("DATE");
            builder.Property(d => d.ValorPrincipal).HasColumnName("valor_principal").HasColumnType("DECIMAL(14,2)").IsRequired();
            builder.Property(d => d.ValorMulta).HasColumnName("valor_multa").HasColumnType("DECIMAL(14,2)").IsRequired();
            builder.Property(d => d.ValorJuros).HasColumnName("valor_juros").HasColumnType("DECIMAL(14,2)").IsRequired();
            builder.Property(d => d.ValorEncargos).HasColumnName("valor_encargos").HasColumnType("DECIMAL(14,2)").IsRequired();
            builder.Property(d => d.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
        }
    }
}
