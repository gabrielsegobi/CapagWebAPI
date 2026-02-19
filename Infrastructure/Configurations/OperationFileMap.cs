using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class OperationFileMap : IEntityTypeConfiguration<OperationFile>
    {
        public void Configure(EntityTypeBuilder<OperationFile> builder)
        {
            builder.ToTable("operation_files");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).HasColumnName("id").HasColumnType("BIGINT").UseMySqlIdentityColumn();
            builder.Property(o => o.FileName).HasColumnName("filename").HasColumnType("VARCHAR(255)");
            builder.Property(o => o.Status).HasColumnName("status").HasColumnType("TINYINT").HasConversion<short>();
            builder.Property(o => o.IdOp).HasColumnName("id_op").HasColumnType("BIGINT");


        }
    }
}

