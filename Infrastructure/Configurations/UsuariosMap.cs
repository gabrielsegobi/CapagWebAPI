using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UsuariosMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(u => u.IdUsuario);

            builder.Property(u => u.IdUsuario).UseMySqlIdentityColumn().HasColumnName("id_usuario").HasColumnType("BIGINT");
            builder.Property(u => u.Email).HasColumnName("email").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(u => u.Nome).HasColumnName("nome").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(u => u.SenhaHash).HasColumnName("senha_hash").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(u => u.Ativo).HasColumnName("ativo").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(u => u.EmailVerificadoEm).HasColumnName("email_verificado_em").HasColumnType("TIMESTAMP");
            builder.Property(ai => ai.UltimoAcesso).HasColumnName("ultimo_acesso").HasColumnType("TIMESTAMP");
            builder.Property(ai => ai.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ai => ai.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ai => ai.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
