using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ProcessLogMap : IEntityTypeConfiguration<ProcessLog>
    {
        public void Configure(EntityTypeBuilder<ProcessLog> builder)
        {
            builder.ToTable("process_log");
            builder.HasKey(pl => pl.Id);

            builder.Property(pl => pl.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(pl => pl.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(pl => pl.Acao).HasColumnName("acao").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(pl => pl.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(pl => pl.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(pl => pl.Mensagem).HasColumnName("mensagem").HasColumnType("TEXT").IsRequired();
        }
    }
}
