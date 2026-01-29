using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SimulacaoIntervaloMap : IEntityTypeConfiguration<SimulacaoIntervalo>
    {
        public void Configure(EntityTypeBuilder<SimulacaoIntervalo> builder)
        {
            builder.ToTable("simulacao_intervalo");

            builder.HasKey(s => s.IdSimulacaoIntervalo);

            builder.Property(s => s.IdSimulacaoIntervalo)
                .HasColumnName("id_simulacao_intervalo")
                .HasColumnType("BIGINT UNSIGNED")
                .UseMySqlIdentityColumn();

            builder.Property(s => s.IdSimulacaoCalc)
                .HasColumnName("id_simulacao_calc")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(s => s.IdTenant)
                .HasColumnName("id_tenant")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(s => s.IdEmpresa)
                .HasColumnName("id_empresa")
                .HasColumnType("BIGINT UNSIGNED")
                .IsRequired();

            builder.Property(s => s.TipoIntervalo)
                .HasColumnName("tipo_intervalo")
                .HasColumnType("ENUM('ENTRADA','PRESTACAO')")
                .IsRequired();

            builder.Property(s => s.MesIni)
                .HasColumnName("mes_ini")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(s => s.MesFim)
                .HasColumnName("mes_fim")
                .HasColumnType("INT")
                .IsRequired();

            builder.Property(s => s.PctMensal)
                .HasColumnName("pct_mensal")
                .HasColumnType("DECIMAL(5,2)");

            builder.Property(s => s.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("TIMESTAMP")
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("TIMESTAMP")
                .IsRequired();

            // 🔗 Relacionamento com simulacao_calc
            builder.HasOne(s => s.SimulacaoCalc)
                .WithMany(c => c.SimulacaoIntervalos)
                .HasForeignKey(s => s.IdSimulacaoCalc)
                .HasConstraintName("fk_simulacao_intervalo_simulacao_calc");
        }
    }
}
