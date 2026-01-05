using Domain.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Views
{
    public class BalancoPatrimonialVwMap : IEntityTypeConfiguration<BalancoPatrimonialVw>
    {
        public void Configure(EntityTypeBuilder<BalancoPatrimonialVw> builder)
        {
            builder.HasNoKey();
            builder.ToView("vw_balanco_patrimonial");

            builder.Property(v => v.Id).HasColumnName("id");
            builder.Property(v => v.IdTenant).HasColumnName("id_tenant");
            builder.Property(v => v.IdEmpresa).HasColumnName("id_empresa");
            builder.Property(v => v.DtIni).HasColumnName("dt_ini");
            builder.Property(v => v.DtIniApur).HasColumnName("dt_ini_apur");
            builder.Property(v => v.DtFinApur).HasColumnName("dt_fin_apur");
            builder.Property(v => v.PerApur).HasColumnName("per_apur");
            builder.Property(v => v.Ano).HasColumnName("ano");
            builder.Property(v => v.Codigo).HasColumnName("codigo");
            builder.Property(v => v.Descricao).HasColumnName("descricao");
            builder.Property(v => v.Tipo).HasColumnName("tipo");
            builder.Property(v => v.Nivel).HasColumnName("nivel");
            builder.Property(v => v.ValCtaRefIni).HasColumnName("val_cta_ref_ini");
            builder.Property(v => v.IndValCtaRefIni).HasColumnName("ind_val_cta_ref_ini");
            builder.Property(v => v.ValCtaRefDeb).HasColumnName("val_cta_ref_deb");
            builder.Property(v => v.ValCtaRefCred).HasColumnName("val_cta_ref_cred");
            builder.Property(v => v.ValCtaRefFin).HasColumnName("val_cta_ref_fin");
            builder.Property(v => v.IndValCtaRefFin).HasColumnName("ind_val_cta_ref_fin");
            builder.Property(v => v.TipoTrib).HasColumnName("tipo_trib");
            builder.Property(v => v.CreatedAt).HasColumnName("created_at");
            builder.Property(v => v.UpdatedAt).HasColumnName("updated_at");
        }
    }
}
