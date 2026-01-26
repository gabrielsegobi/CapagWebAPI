using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegDctfMap : IEntityTypeConfiguration<RegDctf>
    {
        public void Configure(EntityTypeBuilder<RegDctf> builder)
        {
            builder.ToTable("reg_dctf");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(r => r.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.Valor).HasColumnName("valor").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.Periodo).HasColumnName("periodo").HasColumnType("VARCHAR(10)").IsRequired();

            builder.HasOne(rf => rf.FileName).WithMany(r => r.RegDctfs).HasForeignKey(rf => rf.IdFilename);
        }
    }
}
