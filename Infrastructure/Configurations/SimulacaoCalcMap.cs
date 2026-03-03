using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class SimulacaoCalcMap : IEntityTypeConfiguration<SimulacaoCalc>
    {
        public void Configure(EntityTypeBuilder<SimulacaoCalc> builder)
        {
            builder.ToTable("simulacao_calc");
            builder.HasKey(s => s.IdSimulacaoCalc);

            builder.Property(s => s.IdSimulacaoCalc) .HasColumnName("id_simulacao_calc").HasColumnType("BIGINT UNSIGNED").UseMySqlIdentityColumn();
            builder.Property(s => s.IdTenant).HasColumnName("id_tenant") .HasColumnType("BIGINT UNSIGNED") .IsRequired();
            builder.Property(s => s.IdEmpresa).HasColumnName("id_empresa") .HasColumnType("BIGINT UNSIGNED") .IsRequired();
            builder.Property(s => s.TipoSimulacao).HasColumnName("tipo_simulacao").HasColumnType("ENUM('PREVIDENCIARIO', 'OUTRO')").IsRequired();
            builder.Property(s => s.LimitadorPCT).HasColumnName("limitador_pct").HasColumnType("DECIMAL(5,2)").IsRequired();
            builder.Property(s => s.DescMaxPct).HasColumnName("desc_max_pct").HasColumnType("DECIMAL(5,2)");
            builder.Property(s => s.HasPrejuizo).HasColumnName("has_prejuizo").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(s => s.PrejuizoValor).HasColumnName("prejuizo_valor").HasColumnType("DECIMAL(15,2)").IsRequired();
            builder.Property(s => s.HasAbatimento).HasColumnName("has_abatimento") .HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(s => s.AbatimentoValor).HasColumnName("abatimento_valor").HasColumnType("DECIMAL(15,2)").IsRequired();
            builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(s => s.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();

            builder.HasMany(s => s.SimulacaoIntervalos).WithOne(i => i.SimulacaoCalc).HasForeignKey(i => i.IdSimulacaoCalc).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
