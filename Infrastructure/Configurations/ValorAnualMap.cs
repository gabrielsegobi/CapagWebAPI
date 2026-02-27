using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ValorAnualMap : IEntityTypeConfiguration<ValorAnual>
    {
        public void Configure(EntityTypeBuilder<ValorAnual> builder)
        {
            builder.ToTable("valores_anuais");
            builder.HasKey(val => val.IdValor);

            builder.Property(val => val.IdValor).UseMySqlIdentityColumn().HasColumnName("id_valor").HasColumnType("BIGINT");
            builder.Property(val => val.IdIndicador).HasColumnName("id_indicador").HasColumnType("BIGINT").IsRequired();
            builder.Property(val => val.Ano).HasColumnName("ano").HasColumnType("YEAR").IsRequired();
            builder.Property(val => val.Valor).HasColumnName("valor").HasColumnType("DECIMAL(10,2)");
            builder.Property(val => val.ValoresCalcAno).HasColumnName("valores_calc_ano").HasColumnType("VARCHAR(500)");
            builder.Property(val => val.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(val => val.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.HasOne(val => val.Indicador).WithMany(ind => ind.ValoresAnuais).HasForeignKey(val => val.IdIndicador);
        }
    }
}
