using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RegFileNameMap : IEntityTypeConfiguration<RegFileName>
    {
        public void Configure(EntityTypeBuilder<RegFileName> builder)
        {
            builder.ToTable("reg_filename");
            builder.HasKey(rf => rf.Id);

            builder.Property(rf => rf.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(rf => rf.FileName).HasColumnName("filename").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(rf => rf.Type).HasColumnName("type").HasColumnType("VARCHAR(10)").IsRequired();

            //builder.HasMany(rf => rf.RegIrpfs).WithOne(ri => ri.RegFileName).HasForeignKey(ri => ri.IdFilename);
        }
    }
}
