using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegDefisMap : IEntityTypeConfiguration<RegDefi>
    {
        public void Configure(EntityTypeBuilder<RegDefi> builder)
        {
            builder.ToTable("reg_defis");
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(d => d.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(d => d.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(d => d.Periodo).HasColumnName("periodo").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(d => d.Descricao).HasColumnName("descricao").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(d => d.Valor).HasColumnName("valor").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(d => d.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();

            builder.HasOne(er => er.FileName).WithMany(dl => dl.RegDefis).HasForeignKey(er => er.IdFilename);

        }
    }
}
