using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CapagFcoParametroEmpresaMap : IEntityTypeConfiguration<CapagFcoParametroEmpresa>
    {
        public void Configure(EntityTypeBuilder<CapagFcoParametroEmpresa> builder)
        {
            builder.ToTable("fco_parametro_empresa");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT UNSIGNED")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdEmpresa)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(e => e.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(e => e.ExcecoesL100Json)
                .HasColumnName("excecoes_l100_json")
                .HasColumnType("json")
                .IsRequired();

            builder.Property(e => e.ExcecoesL300Json)
                .HasColumnName("excecoes_l300_json")
                .HasColumnType("json")
                .IsRequired();

            builder.Property(e => e.BaseVersaoHash)
                .HasColumnName("base_versao_hash")
                .HasColumnType("VARCHAR(16)");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario")
                .HasColumnType("BIGINT");

            builder.HasIndex(e => e.IdEmpresa).IsUnique();
        }
    }
}
