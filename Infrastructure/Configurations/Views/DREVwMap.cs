using Domain.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Views
{
    public class DREVwMap : IEntityTypeConfiguration<DREVw>
    {
        public void Configure(EntityTypeBuilder<DREVw> builder)
        {
            builder.ToView("vw_dre");
            builder.HasNoKey();

            builder.Property(v => v.Id).HasColumnName("id");
            builder.Property(v => v.IdTenant).HasColumnName("id_tenant");
            builder.Property(v => v.IdEmpresa).HasColumnName("id_empresa");
            builder.Property(v => v.DtIni).HasColumnName("dt_ini");
            builder.Property(v => v.DtIniApur).HasColumnName("dt_ini_apur");
            builder.Property(v => v.DtFinApur).HasColumnName("dt_fin_apur");
            builder.Property(v => v.PerApur).HasColumnName("per_apur").HasMaxLength(10);
            builder.Property(v => v.Ano).HasColumnName("ano");
            builder.Property(v => v.Codigo).HasColumnName("codigo").HasMaxLength(20);
            builder.Property(v => v.Descricao).HasColumnName("descricao").HasMaxLength(255);
            builder.Property(v => v.Tipo).HasColumnName("tipo").HasColumnType("char(1)");
            builder.Property(v => v.Nivel).HasColumnName("nivel");
            builder.Property(v => v.ValCtaRefIni).HasColumnName("val_cta_ref_ini").HasColumnType("decimal(18,2)");
            builder.Property(v => v.IndValCtaRefIni).HasColumnName("ind_val_cta_ref_ini").HasColumnType("char(1)");
            builder.Property(v => v.ValCtaRefDeb).HasColumnName("val_cta_ref_deb").HasColumnType("decimal(18,2)");
            builder.Property(v => v.ValCtaRefCred).HasColumnName("val_cta_ref_cred").HasColumnType("decimal(18,2)");
            builder.Property(v => v.ValCtaRefFin).HasColumnName("val_cta_ref_fin").HasColumnType("decimal(18,2)");
            builder.Property(v => v.IndValCtaRefFin).HasColumnName("ind_val_cta_ref_fin").HasColumnType("char(1)");
            builder.Property(v => v.TipoTrib).HasColumnName("tipo_trib").HasMaxLength(50);
            builder.Property(v => v.CreatedAt).HasColumnName("created_at");
            builder.Property(v => v.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
