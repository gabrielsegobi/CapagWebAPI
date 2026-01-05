using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AnaliseICPMap : IEntityTypeConfiguration<AnaliseICP>
    {
        public void Configure(EntityTypeBuilder<AnaliseICP> builder)
        {
            builder.ToTable("analises_icp");
            builder.HasKey(ai => ai.IdAnalise);

            builder.Property(ai => ai.IdAnalise).UseMySqlIdentityColumn().HasColumnName("id_analise").HasColumnType("BIGINT");
            builder.Property(ai => ai.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(ai => ai.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(ai => ai.IcpCalculado).HasColumnName("icp_calculado").HasColumnType("DECIMAL(7,4)");
            builder.Property(ai => ai.Classificacao).HasColumnName("classificacao").HasColumnType("CHAR(1)");
            builder.Property(ai => ai.SomaPesos).HasColumnName("soma_pesos").HasColumnType("DECIMAL(7,4)").IsRequired();
            builder.Property(ai => ai.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ai => ai.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ai => ai.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
