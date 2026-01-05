using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");
            builder.HasKey(rt => rt.IdRefreshToken);

            builder.Property(rt => rt.IdRefreshToken).UseMySqlIdentityColumn().HasColumnName("id_refresh_token").HasColumnType("BIGINT");
            builder.Property(rt => rt.IdUsuario).HasColumnName("id_usuario").HasColumnType("BIGINT").IsRequired();
            builder.Property(rt => rt.Token).HasColumnName("token").HasColumnType("VARCHAR(500)").IsRequired();
            builder.Property(rt => rt.DeviceId).HasColumnName("device_id").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(rt => rt.ExpiraEm).HasColumnName("expira_em").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rt => rt.RevogadoEm).HasColumnName("revogado_em").HasColumnType("TIMESTAMP");
            builder.Property(rt => rt.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rt => rt.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
        }
    }
}
