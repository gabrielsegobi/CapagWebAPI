using Domain.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Views
{
    public class UsuariosAcessosVwMap : IEntityTypeConfiguration<UsuariosAcessosVw>
    {
        public void Configure(EntityTypeBuilder<UsuariosAcessosVw> builder)
        {
            builder.ToView("vw_usuario_acessos");
            builder.HasNoKey(); 

            builder.Property(v => v.StatusTenant).HasColumnName("status_tenant");
            builder.Property(v => v.SlugTenant).HasColumnName("slug_tenant");
            builder.Property(v => v.Papel).HasColumnName("papel");
            builder.Property(v => v.NomeUsuario).HasColumnName("nome_usuario");
            builder.Property(v => v.NomeTenant).HasColumnName("nome_tenant");
            builder.Property(v => v.IdUsuario).HasColumnName("id_usuario");
            builder.Property(v => v.IdTenant).HasColumnName("id_tenant");
            builder.Property(v => v.Email).HasColumnName("email");
        }
    }
}
