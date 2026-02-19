using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class OperationMap : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.ToTable("operation");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).HasColumnName("id").HasColumnType("BIGINT").UseMySqlIdentityColumn();
            builder.Property(o => o.DateCreate).HasColumnName("date_create").HasColumnType("DATETIME").IsRequired();
            builder.Property(o => o.DateStart).HasColumnName("date_start").HasColumnType("DATETIME");
            builder.Property(o => o.DateFinish).HasColumnName("date_finish").HasColumnType("DATETIME");
            builder.Property(o => o.OperationName).HasColumnName("operation").HasColumnType("VARCHAR(255)");
            builder.Property(o => o.Status).HasColumnName("status").HasColumnType("TINYINT").HasConversion<short>();
            builder.Property(o => o.IdUsuario).HasColumnName("id_usuario").HasColumnType("BIGINT").IsRequired();
            builder.Property(o => o.Cnpj).HasColumnName("cnpj").HasColumnType("VARCHAR(20)");
            builder.Property(o => o.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(o => o.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();

            // ========= Relacionamentos =========

            builder.HasOne(o => o.Empresa).WithMany().HasForeignKey(o => o.IdEmpresa);
            builder.HasOne(o => o.Tenant).WithMany().HasForeignKey(o => o.IdTenant);
            builder.HasMany(o => o.Files).WithOne(f => f.Operation).HasForeignKey(f => f.IdOp).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
