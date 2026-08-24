using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class CapagCalculadoraResultadoMap : IEntityTypeConfiguration<CapagCalculadoraResultado>
    {
        public void Configure(EntityTypeBuilder<CapagCalculadoraResultado> builder)
        {
            builder.ToTable("capag_calculadora_resultado");
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

            builder.Property(e => e.Modelo)
                .HasColumnName("modelo")
                .HasColumnType("ENUM('capag-e-1','capag-e-2','capag-p')")
                .IsRequired();

            builder.Property(e => e.Classificacao)
                .HasColumnName("classificacao")
                .HasColumnType("CHAR(1)")
                .IsRequired();

            builder.Property(e => e.PercentualExibicao)
                .HasColumnName("percentual_exibicao")
                .HasColumnType("VARCHAR(16)")
                .IsRequired();

            builder.Property(e => e.LabelMetrica)
                .HasColumnName("label_metrica")
                .HasColumnType("VARCHAR(64)")
                .IsRequired();

            builder.Property(e => e.StatusMensagem)
                .HasColumnName("status_mensagem")
                .HasColumnType("VARCHAR(255)")
                .IsRequired();

            builder.Property(e => e.ValorCapag)
                .HasColumnName("valor_capag")
                .HasColumnType("DECIMAL(20,4)")
                .IsRequired();

            builder.Property(e => e.ValorDivida)
                .HasColumnName("valor_divida")
                .HasColumnType("DECIMAL(20,4)");

            builder.Property(e => e.Indice)
                .HasColumnName("indice")
                .HasColumnType("DECIMAL(18,6)");

            builder.Property(e => e.Parcial)
                .HasColumnName("parcial")
                .HasColumnType("TINYINT(1)")
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.DateCreate)
                .HasColumnName("date_create")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.DateUpdate)
                .HasColumnName("date_update")
                .HasColumnType("DATETIME")
                .IsRequired();

            builder.Property(e => e.IdUsuario)
                .HasColumnName("id_usuario")
                .HasColumnType("BIGINT");
        }
    }
}
