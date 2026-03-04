using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrastructure.Configurations
{
    public class IndicadorMap : IEntityTypeConfiguration<Indicador>
    {
        public void Configure(EntityTypeBuilder<Indicador> builder)
        {
            builder.ToTable("indicadores");
            builder.HasKey(ir => ir.IdIndicador);

            builder.Property(ir => ir.IdIndicador).UseMySqlIdentityColumn().HasColumnName("id_indicador").HasColumnType("BIGINT");
            builder.Property(ir => ir.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(ir => ir.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(ir => ir.Nome).HasColumnName("nome").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(ir => ir.ValoresCalcSaudeEmpresa).HasColumnName("valores_calc_saude_empresa").HasColumnType("VARCHAR(500)");
            builder.Property(ir => ir.SaudeEmpresa).HasColumnName("saude_empresa").HasColumnType("DECIMAL(12,4)");
            builder.Property(ir => ir.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ir => ir.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(ir => ir.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");

            builder.HasMany(ind => ind.ValoresAnuais).WithOne(val => val.Indicador).HasForeignKey(val => val.IdIndicador);
        }
    }
}
