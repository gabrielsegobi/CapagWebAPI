using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ICPLimitMap : IEntityTypeConfiguration<ICPLimit>
    {
        public void Configure(EntityTypeBuilder<ICPLimit> builder)
        {
            builder.ToTable("icp_limits");
            builder.HasKey(il => il.Id);

            builder.Property(il => il.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(il => il.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(il => il.Label).HasColumnName("label").HasColumnType("VARCHAR(120)");
            builder.Property(il => il.MinValue).HasColumnName("min_value").HasColumnType("DECIMAL(10,4)").IsRequired();
            builder.Property(il => il.MaxValue).HasColumnName("max_value").HasColumnType("DECIMAL(10,4)").IsRequired();
            builder.Property(il => il.SortOrder).HasColumnName("sort_order").HasColumnType("TINYINT UNSIGNED");
            builder.Property(il => il.ColorCode).HasColumnName("color_code").HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(il => il.Active).HasColumnName("active").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(il => il.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(il => il.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
        }
    }
}
