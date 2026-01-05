using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UsuarioTenantMap : IEntityTypeConfiguration<UsuarioTenant>
    {
        public void Configure(EntityTypeBuilder<UsuarioTenant> builder)
        {
            builder.ToTable("usuario_tenant");
            builder.HasKey(ai => ai.Id);

            builder.Property(ai => ai.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(ai => ai.IdUsuario).HasColumnName("id_usuario").HasColumnType("BIGINT").IsRequired();
            builder.Property(ai => ai.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            //builder.Property(al => al.Papel).HasColumnName("papel").HasColumnType("ENUM('proprietario','administrador','editor','visualizador')").HasConversion(
            //v => v.ToString(),
            //v => (PapelUsuarioTenantEnum)Enum.Parse(typeof(PapelUsuarioTenantEnum), v)).IsRequired();
            builder.Property(ai => ai.Papel).HasColumnName("papel").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(dc => dc.Ativo).HasColumnName("ativo").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(ai => ai.DataVinculo).HasColumnName("data_vinculo").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ai => ai.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
