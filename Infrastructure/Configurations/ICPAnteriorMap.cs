using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ICPAnteriorMap : IEntityTypeConfiguration<ICPsAnterior>
    {
        public void Configure(EntityTypeBuilder<ICPsAnterior> builder)
        {
            builder.ToTable("icp_anterior");
            builder.HasKey(ia => ia.IdIcpAnterior);

            builder.Property(r => r.IdIcpAnterior).UseMySqlIdentityColumn().HasColumnName("id_icp_anterior").HasColumnType("BIGINT");
            builder.Property(r => r.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.Classificacao).HasColumnName("classificacao").HasColumnType("CHAR(1)");
            builder.Property(r => r.ValorICPReceita).HasColumnName("valor_icp_receita").HasColumnType("DECIMAL(12,2)").IsRequired();
            builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
        }
    }
}
