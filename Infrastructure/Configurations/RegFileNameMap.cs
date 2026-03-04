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
            builder.Property(rf => rf.Status).HasColumnName("status").HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(rf => rf.Error).HasColumnName("error").HasColumnType("VARCHAR(150)").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();

            builder.HasMany(rf => rf.RegDefis).WithOne(ri => ri.FileName).HasForeignKey(ri => ri.IdFilename);
            builder.HasMany(rf => rf.RegDarfs).WithOne(rda => rda.FileName).HasForeignKey(rda => rda.IdFilename);
            builder.HasMany(rf => rf.RegDctfs).WithOne(rdc => rdc.FileName).HasForeignKey(rdc => rdc.IdFilename);
            builder.HasMany(rf => rf.RegDirfTerceiros).WithOne(rdt => rdt.FileName).HasForeignKey(rdt => rdt.IdFilename);
            builder.HasMany(rf => rf.RegIrpfs).WithOne(rip => rip.FileName).HasForeignKey(rip => rip.IdFilename);
            builder.HasMany(rf => rf.RegPgdasds).WithOne(rp => rp.FileName).HasForeignKey(rp => rp.IdFilename);
        }
    }
}
