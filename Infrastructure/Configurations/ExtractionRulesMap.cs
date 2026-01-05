using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ExtractionRulesMap : IEntityTypeConfiguration<ExtractionRule>
    {
        public void Configure(EntityTypeBuilder<ExtractionRule> builder)
        {
            builder.ToTable("extraction_rules");
            builder.HasKey(er => er.Id);

            builder.Property(er => er.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(er => er.LayoutId).HasColumnName("layout_id").HasColumnType("BIGINT").IsRequired();
            builder.Property(er => er.FieldLabel).HasColumnName("field_label").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(er => er.ExtractionRegex).HasColumnName("extraction_regex").HasColumnType("TEXT").IsRequired();
            builder.Property(er => er.RegexGroupIndex).HasColumnName("regex_group_index").HasColumnType("INT");
            builder.Property(er => er.DestinationTable).HasColumnName("destination_table").HasColumnType("VARCHAR(64)").IsRequired();
            builder.Property(er => er.DestinationColumn).HasColumnName("destination_column").HasColumnType("VARCHAR(64)").IsRequired();
            builder.Property(er => er.DataType).HasColumnName("data_type").HasColumnType("VARCHAR(50)");

            builder.HasOne(er => er.Layout).WithMany(dl => dl.ExtractionRules).HasForeignKey(er => er.LayoutId);
        }
    }
}
