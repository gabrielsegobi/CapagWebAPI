using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegimeTributarioMap : IEntityTypeConfiguration<RegimeTributario>
    {
        public void Configure(EntityTypeBuilder<RegimeTributario> builder)
        {
            builder.ToTable("regimes_tributarios");
            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(rt => rt.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(rt => rt.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(rt => rt.DtIni).HasColumnName("dt_ini").HasColumnType("DATE");
            builder.Property(rt => rt.Ano).HasColumnName("ano").HasColumnType("INT").IsRequired();
            builder.Property(rt => rt.RaizCnpj).HasColumnName("raiz_cnpj").HasColumnType("VARCHAR(8)").IsRequired();
            builder.Property(rt => rt.FormaTribCompleta).HasColumnName("forma_trib_completa").HasColumnType("VARCHAR(100)");
            builder.Property(rt => rt.FormaApurCompleta).HasColumnName("forma_apur_completa").HasColumnType("VARCHAR(100)");
            builder.Property(rt => rt.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rt => rt.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rt => rt.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
