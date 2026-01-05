using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DocumentLayoutsMap : IEntityTypeConfiguration<DocumentLayout>
    {
        public void Configure(EntityTypeBuilder<DocumentLayout> builder)
        {
            builder.ToTable("document_layouts");
            builder.HasKey(dl => dl.Id);

            builder.Property(dl => dl.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(dl => dl.LayoutName).HasColumnName("layout_name").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(dl => dl.Description).HasColumnName("description").HasColumnType("VARCHAR(255)");
            builder.Property(dl => dl.ValidationRegex).HasColumnName("validation_regex").HasColumnType("TEXT").IsRequired();
            builder.Property(dl => dl.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP");
            builder.Property(dl => dl.Active).HasColumnName("active").HasColumnType("TINYINT(1)");

            builder.HasMany(dl => dl.ExtractionRules).WithOne(er => er.Layout).HasForeignKey(er => er.LayoutId);
        }
    }
}
