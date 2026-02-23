using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ValidationRegexMap : IEntityTypeConfiguration<ValidationRegex>
    {
        public void Configure(EntityTypeBuilder<ValidationRegex> builder)
        {
            builder.ToTable("validation_regex");
            builder.HasKey(vr => vr.Id);

            builder.Property(vr => vr.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(vr => vr.Regex).HasColumnName("regex").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(vr => vr.DocumentLayoutId).HasColumnName("document_layouts_id").HasColumnType("BIGINT").IsRequired();

            builder.HasOne(vr => vr.Layout).WithMany(dl => dl.ValidationRegexes).HasForeignKey(vr => vr.DocumentLayoutId);
        }
    }
}
