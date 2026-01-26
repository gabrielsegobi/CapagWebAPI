using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ValorCalcVariavelMap : IEntityTypeConfiguration<ValorCalcVariavel>
    {
        public void Configure(EntityTypeBuilder<ValorCalcVariavel> builder)
        {
            builder.ToTable("valor_calc_variavel");
            builder.HasKey(v => v.IdValorCalcVariavel);

            builder.Property(v => v.IdValorCalcVariavel).HasColumnName("id_valor_calc_variavel").HasColumnType("BIGINT").UseMySqlIdentityColumn();
            builder.Property(v => v.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(v => v.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(v => v.IdTipoGrupo).HasColumnName("id_tipo_grupo").HasColumnType("BIGINT").IsRequired();
            builder.Property(v => v.AnoBase).HasColumnName("ano_base").HasColumnType("YEAR").IsRequired();
            builder.Property(v => v.IdVariavel).HasColumnName("id_variavel").HasColumnType("VARCHAR(3)").IsRequired();
            builder.Property(v => v.Valor).HasColumnName("valor").HasColumnType("DECIMAL(12,4)").IsRequired();
            builder.Property(v => v.Status).HasColumnName("status").HasColumnType("ENUM('preenchido','calculado')").IsRequired();
            builder.Property(v => v.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(v => v.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
        }
    }
}
