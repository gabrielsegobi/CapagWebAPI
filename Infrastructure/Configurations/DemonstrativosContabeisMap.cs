using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DemonstrativosContabeisMap : IEntityTypeConfiguration<DemonstrativoContabil>
    {
        public void Configure(EntityTypeBuilder<DemonstrativoContabil> builder)
        {
            builder.ToTable("demonstrativos_contabeis");
            builder.HasKey(dc => new { dc.Id, dc.IdTenant });

            builder.Property(dc => dc.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(dc => dc.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(dc => dc.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(dc => dc.DtIni).HasColumnName("dt_ini").HasColumnType("DATE");
            builder.Property(dc => dc.DtIniApur).HasColumnName("dt_ini_apur").HasColumnType("DATE");
            builder.Property(dc => dc.DtFinApur).HasColumnName("dt_fin_apur").HasColumnType("DATE");
            builder.Property(dc => dc.PerApur).HasColumnName("per_apur").HasColumnType("VARCHAR(10)");
            builder.Property(dc => dc.Ano).HasColumnName("ano").HasColumnType("INT").IsRequired();
            builder.Property(dc => dc.Codigo).HasColumnName("codigo").HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(dc => dc.Descricao).HasColumnName("descricao").HasColumnType("VARCHAR(255)");
            builder.Property(dc => dc.Tipo).HasColumnName("tipo").HasColumnType("CHAR(1)");
            builder.Property(dc => dc.Nivel).HasColumnName("nivel").HasColumnType("TINYINT");
            builder.Property(dc => dc.ValCtaRefIni).HasColumnName("val_cta_ref_ini").HasColumnType("DECIMAL(18,2)");
            builder.Property(dc => dc.IndValCtaRefIni).HasColumnName("ind_val_cta_ref_ini").HasColumnType("CHAR(1)");
            builder.Property(dc => dc.ValCtaRefDeb).HasColumnName("val_cta_ref_deb").HasColumnType("DECIMAL(18,2)");
            builder.Property(dc => dc.ValCtaRefCred).HasColumnName("val_cta_ref_cred").HasColumnType("DECIMAL(18,2)");
            builder.Property(dc => dc.ValCtaRefFin).HasColumnName("val_cta_ref_fin").HasColumnType("DECIMAL(18,2)");
            builder.Property(dc => dc.IndValCtaRefFin).HasColumnName("ind_val_cta_ref_fin").HasColumnType("CHAR(1)");
            builder.Property(dc => dc.TipoTrib).HasColumnName("tipo_trib").HasColumnType("VARCHAR(50)");
            builder.Property(dc => dc.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(dc => dc.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(dc => dc.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
