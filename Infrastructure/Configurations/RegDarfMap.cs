using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegDarfMap : IEntityTypeConfiguration<RegDarfs>
    {
        public void Configure(EntityTypeBuilder<RegDarfs> builder)
        {
            builder.ToTable("reg_darf");
            builder.HasKey(rd => rd.Id);

            builder.Property(rd => rd.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(rd => rd.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(rd => rd.IdFilename).HasColumnName("id_filename").HasColumnType("BIGINT").IsRequired();
            builder.Property(rd => rd.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(rd => rd.DataArrecadacao).HasColumnName("data_arrecadacao").HasColumnType("DATE").IsRequired();
            builder.Property(rd => rd.ValorTotal).HasColumnName("valor_total").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(rd => rd.DataProcessamento).HasColumnName("data_processamento").HasColumnType("TIMESTAMP");
        }
    }
}

