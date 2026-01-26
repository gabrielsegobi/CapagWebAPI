using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegDirfTerceiroMap : IEntityTypeConfiguration<RegDirfTerceiro>
    {
        public void Configure(EntityTypeBuilder<RegDirfTerceiro> builder)
        {
            builder.ToTable("reg_dirf_terceiro");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).UseMySqlIdentityColumn().HasColumnName("id") .HasColumnType("BIGINT");
            builder.Property(r => r.IdTenant).HasColumnName("id_tenant") .HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();
            builder.Property(r => r.DataProcessamento).HasColumnName("data_processamento").HasColumnType("DATE").IsRequired();
            builder.Property(r => r.Codigo).HasColumnName("codigo").HasColumnType("VARCHAR(10)").IsRequired();
            builder.Property(r => r.ValorRendimento).HasColumnName("valor_rendimento").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.ValorTributo).HasColumnName("valor_tributo").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(r => r.AnoCalendario).HasColumnName("ano_calendario").HasColumnType("VARCHAR(4)").IsRequired();


            builder.HasOne(rf => rf.FileName).WithMany(r => r.RegDirfTerceiros).HasForeignKey(rf => rf.IdFilename);
        }
    }
}
