using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class TenantsMap : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("tenants");
            builder.HasKey(t => t.IdTenant);

            builder.Property(t => t.IdTenant).UseMySqlIdentityColumn().HasColumnName("id_tenant").HasColumnType("BIGINT");
            builder.Property(t => t.Nome).HasColumnName("nome").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(t => t.Slug).HasColumnName("slug").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(t => t.Status).HasColumnName("status").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(t => t.Plano).HasColumnName("plano").HasColumnType("VARCHAR(50)").IsRequired();
            //builder.Property(t => t.Status).HasColumnName("status").HasColumnType("ENUM('ativo','suspenso','cancelado','trial')").HasConversion(
            //v => v.ToString(),
            //v => (TenantStatusEnum)Enum.Parse(typeof(TenantStatusEnum), v)).IsRequired();
            //builder.Property(t => t.Plano).HasColumnName("plano").HasColumnType("ENUM('basico','profissional','enterprise')").HasConversion(
            //v => v.ToString(),
            //v => (TenantPlanosEnum)Enum.Parse(typeof(TenantPlanosEnum), v)).IsRequired();
            builder.Property(t => t.DataCriacao).HasColumnName("data_criacao").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(t => t.DataExpiracao).HasColumnName("data_expiracao").HasColumnType("TIMESTAMP");
            builder.Property(t => t.Configuracoes).HasColumnName("configuracoes").HasColumnType("json");
            builder.Property(t => t.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
