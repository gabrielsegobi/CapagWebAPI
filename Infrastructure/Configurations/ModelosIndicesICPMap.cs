using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ModelosIndicesICPMap : IEntityTypeConfiguration<ModeloIndiceICP>
    {
        public void Configure(EntityTypeBuilder<ModeloIndiceICP> builder)
        {
            builder.ToTable("modelos_indices_icp");
            builder.HasKey(mi => mi.IdModeloIndice);

            builder.Property(mi => mi.IdModeloIndice).UseMySqlIdentityColumn().HasColumnName("id_modelo_indice").HasColumnType("BIGINT");
            builder.Property(mi => mi.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT");
            builder.Property(mi => mi.Nome).HasColumnName("nome").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(mi => mi.Formula).HasColumnName("formula").HasColumnType("VARCHAR(255)");
            builder.Property(mi => mi.Meta).HasColumnName("meta").HasColumnType("DECIMAL(12,2)");
            builder.Property(mi => mi.PiorCaso).HasColumnName("pior_caso").HasColumnType("DECIMAL(12,2)");
            builder.Property(mi => mi.Peso).HasColumnName("peso").HasColumnType("DECIMAL(5,4)").IsRequired();
            builder.Property(mi => mi.Ativo).HasColumnName("ativo").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(mi => mi.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(mi => mi.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(mi => mi.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");

            builder.HasMany(mi => mi.Resultados).WithOne(r => r.ModeloIndice).HasForeignKey(r => r.IdModeloIndice);

        }
    }
}
