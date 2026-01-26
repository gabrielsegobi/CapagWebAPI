using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegPgdasdMap : IEntityTypeConfiguration<RegPgdasd>
    {
        public void Configure(EntityTypeBuilder<RegPgdasd> builder)
        {
            builder.ToTable("reg_pgdasd");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnName("id").HasColumnType("BIGINT").UseMySqlIdentityColumn();
            builder.Property(r => r.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.Periodo).HasColumnName("periodo").HasColumnType("VARCHAR(10)").IsRequired();
            builder.Property(r => r.ReceitaBruta).HasColumnName("receita_bruta").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.TotalDebito).HasColumnName("total_debito").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();

            builder.HasOne(rf => rf.FileName).WithMany(r => r.RegPgdasds).HasForeignKey(rf => rf.IdFilename);
        }
    }
}
