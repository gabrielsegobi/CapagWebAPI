using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_L030Map : IEntityTypeConfiguration<E_L030>
    {
        public void Configure(EntityTypeBuilder<E_L030> builder)
        {
            builder.ToTable("ecf_l030");

            builder.HasKey(e => new { e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            //builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.FileName).HasColumnName("file_name").HasColumnType("VARCHAR(255)");

            // ===== E_L030 =====
            builder.Property(e => e.Reg).HasColumnName("reg").HasColumnType("TEXT");
            builder.Property(e => e.DtIni).HasColumnName("dt_ini").HasColumnType("TEXT");
            builder.Property(e => e.DtFin).HasColumnName("dt_fin").HasColumnType("TEXT");
            builder.Property(e => e.PerApur).HasColumnName("per_apur").HasColumnType("TEXT");
        }
    }
}
