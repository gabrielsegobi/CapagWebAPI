using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_P150Map : IEntityTypeConfiguration<E_P150>
    {
        public void Configure(EntityTypeBuilder<E_P150> builder)
        {
            builder.ToTable("ecf_p150");

            builder.HasKey(e => new { e.IdOp, e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileName).HasColumnName("file_name").HasColumnType("TEXT");

            // ===== E_P150 =====
            builder.Property(e => e.Reg).HasColumnName("reg").HasColumnType("TEXT");
            builder.Property(e => e.Codigo).HasColumnName("codigo").HasColumnType("TEXT");
            builder.Property(e => e.Descricao).HasColumnName("descricao").HasColumnType("TEXT");
            builder.Property(e => e.Tipo).HasColumnName("tipo").HasColumnType("TEXT");
            builder.Property(e => e.Nivel).HasColumnName("nivel").HasColumnType("TEXT");
            builder.Property(e => e.CodNat).HasColumnName("cod_nat").HasColumnType("TEXT");
            builder.Property(e => e.CodCtaSup).HasColumnName("cod_cta_sup").HasColumnType("TEXT");
            builder.Property(e => e.Valor).HasColumnName("valor").HasColumnType("TEXT");
            builder.Property(e => e.IndValor).HasColumnName("ind_valor").HasColumnType("TEXT");
        }
    }
}
