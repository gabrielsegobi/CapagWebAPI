using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_L300Map : IEntityTypeConfiguration<E_L300>
    {
        public void Configure(EntityTypeBuilder<E_L300> builder)
        {
            builder.ToTable("ecf_l300");

            builder.HasKey(e => new { e.IdOp, e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileName).HasColumnName("filename").HasColumnType("VARCHAR(255)");

            // ===== E_L300 =====
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
