using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_0001Map : IEntityTypeConfiguration<E_0001>
    {
        public void Configure(EntityTypeBuilder<E_0001> builder)
        {
            builder.ToTable("ecf_0001");

            builder.HasKey(e => new { e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            //builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.FileName).HasColumnName("filename").HasColumnType("TEXT");

            // ===== E_0001 =====
            builder.Property(e => e.Reg).HasColumnName("reg").HasColumnType("TEXT");
            builder.Property(e => e.IndDad).HasColumnName("ind_dad").HasColumnType("TEXT");
        }
    }
}
