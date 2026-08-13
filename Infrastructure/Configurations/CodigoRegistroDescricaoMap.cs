using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CodigoRegistroDescricaoMap : IEntityTypeConfiguration<CodigoRegistroDescricao>
    {
        public void Configure(EntityTypeBuilder<CodigoRegistroDescricao> builder)
        {
            builder.ToTable("codigos_registro_descricao");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("BIGINT")
                .UseMySqlIdentityColumn();

            builder.Property(e => e.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.IdEmpresa)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT")
                .IsRequired();

            builder.Property(e => e.Codigo)
                .HasColumnName("codigo")
                .HasColumnType("VARCHAR(20)")
                .IsRequired();

            builder.Property(e => e.ExpressaoRegular)
                .HasColumnName("expressao_regular")
                .HasColumnType("VARCHAR(150)")
                .IsRequired()
                .HasDefaultValue(string.Empty);

            builder.Property(e => e.IsValid)
                .HasColumnName("is_valid")
                .HasColumnType("TINYINT(1)")
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
