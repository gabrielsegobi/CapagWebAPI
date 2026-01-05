using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ResultadosIndicesICPMap : IEntityTypeConfiguration<ResultadoIndiceICP>
    {
        public void Configure(EntityTypeBuilder<ResultadoIndiceICP> builder)
        {
            builder.ToTable("resultados_indices_icp");
            builder.HasKey(ri => ri.IdResultadoIndice);

            builder.Property(ri => ri.IdResultadoIndice).UseMySqlIdentityColumn().HasColumnName("id_resultado_indice").HasColumnType("BIGINT");
            builder.Property(ri => ri.IdModeloIndice).HasColumnName("id_modelo_indice").HasColumnType("BIGINT").IsRequired();
            builder.Property(ri => ri.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();

            builder.Property(ri => ri.ValorCalculado).HasColumnName("valor_calculado").HasColumnType("DECIMAL(12,2)");
            builder.Property(ri => ri.SubScoreNormalizado).HasColumnName("sub_score_normalizado").HasColumnType("DECIMAL(7,4)");
            builder.Property(ri => ri.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ri => ri.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ri => ri.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();

            builder.HasOne(ri => ri.ModeloIndice).WithMany(mi => mi.Resultados).HasForeignKey(ri => ri.IdModeloIndice);
        }
    }
}
