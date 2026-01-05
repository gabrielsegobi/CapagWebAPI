using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegIrpfMap : IEntityTypeConfiguration<RegIrpf>
    {
        public void Configure(EntityTypeBuilder<RegIrpf> builder)
        {
            builder.ToTable("reg_irpf");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("id") .HasColumnType("BIGINT") .UseMySqlIdentityColumn();
            builder.Property(r => r.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.ValorV1).HasColumnName("valor_v1").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.ValorV2).HasColumnName("valor_v2").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.ValorV3).HasColumnName("valor_v3").HasColumnType("DECIMAL(12,4)").IsRequired(); 
            builder.Property(r => r.ValorV4).HasColumnName("valor_v4").HasColumnType("DECIMAL(12,4)").IsRequired(); 
            builder.Property(r => r.ValorV6).HasColumnName("valor_v6").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.ValorV7).HasColumnName("valor_v7").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.AnoCalendario).HasColumnName("ano_calendario").HasColumnType("VARCHAR(4)").IsRequired();

            //builder.HasOne(r => r.RegFileName).WithMany(f => f.RegIrpfs).HasForeignKey(r => r.IdFilename);
        }
    }
}
