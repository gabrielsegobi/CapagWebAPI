using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TipoGrupoMap : IEntityTypeConfiguration<TipoGrupo>
    {
        public void Configure(EntityTypeBuilder<TipoGrupo> builder)
        {
            builder.ToTable("tipo_grupo");
            builder.HasKey(tg => tg.Id);

            builder.Property(tg => tg.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(tg => tg.Name).HasColumnName("Name").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(tg => tg.Tag).HasColumnName("Tag").HasColumnType("VARCHAR(40)").IsRequired();
        }
    }
}
